using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using Application.Posts.RepositoryContracts;
using Application.Posts.Entities;

namespace Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private string rootDirectory;

        public void SetRootDirectory( string directory )
        {
            rootDirectory = directory;
        }

        public void CreatePost( Post post )
        {
            CreateAuthorDirectory( post.author );
            var fullFilePath = string.Format( "{0}\\{1}\\{2}",
                rootDirectory, AuthorDirectory( post.author ), FileName( post.date, post.title ) );
            WriteToFile( fullFilePath, post.tags, post.body );
        }

        public void DeletePost( string author, DateTime date, string title )
        {
            var fullFilePath = string.Format( "{0}\\{1}\\{2}",
                rootDirectory, AuthorDirectory( author ), FileName( date, title ) );
            File.Delete( fullFilePath );
        }

        public void CreateAuthorDirectory( string author )
        {
            var directory = string.Format( "{0}\\{1}", rootDirectory, AuthorDirectory( author ) );
            Directory.CreateDirectory( directory );
        }

        private string AuthorDirectory( string author )
        {
            return author.ToLower().Replace( " ", "_" );
        }

        public string FileName( DateTime date, string title )
        {
            var date_path = date.ToString( "yyyy_MM_dd" );
            var title_path = title.Replace( " ", "_" );
            var fileName = string.Format( "{0}_{1}.html", date_path, title_path );
            return fileName;
        }

        private void WriteToFile( string fullFilePath, string[] tags, string body )
        {
            using ( var fileStream = File.Create( fullFilePath ) )
            {
                WriteTags( fileStream, tags );
                WriteStringToFile( fileStream, body );
            }
        }

        private void WriteTags( FileStream fileStream, string[] tags )
        {
            foreach( string tag in tags )
            {
                WriteStringToFile( fileStream, tag + ":" );
            }
            WriteStringToFile( fileStream, Environment.NewLine );
            WriteStringToFile( fileStream, Environment.NewLine );
        }

        private void WriteStringToFile( FileStream fileStream, string text )
        {
            foreach( byte b in GetBytes( text ) )
                fileStream.WriteByte( b );
        }

        private byte[] GetBytes( string str )
        {
            byte[] bytes = new byte[str.Length * sizeof( char )];
            System.Buffer.BlockCopy( str.ToCharArray(), 0, bytes, 0, bytes.Length );
            return bytes;
        }
        
        public IEnumerable<Post> GetAllPosts( string author )
        {
            var posts = new List<Post>();
            var path = string.Format( "{0}\\{1}", rootDirectory, AuthorDirectory( author ) );
            var allPostPaths = Directory.GetFiles( path );
            foreach( var postPath in allPostPaths )
            {
                posts.Add( MapFileToPost( postPath ) );
            }
            return posts;
        }

        public IEnumerable<Post> GetAllPosts()
        {
            var allPosts = new List<Post>();
            foreach( var postPath in GetAllFiles() )
            {
                allPosts.Add( MapFileToPost( postPath ) );
            }
            return allPosts;
        }

        private List<string> GetAllFiles()
        {
            var files = new List<string>();
            foreach( string authors in Directory.GetDirectories( rootDirectory ) )
            {
                foreach( string file in Directory.GetFiles( authors ) )
                {
                    files.Add( file );
                }
            }
            return files;
        }

        public Post MapFileToPost( string filePath )
        {
            byte[] readBuffer = File.ReadAllBytes( filePath );
            var fileContents = GetString( readBuffer );
            var post = new Post
            {
                author = GetAuthor( filePath ),
                date = GetDate( filePath ),
                title = GetTitle( filePath ),
                body = GetBody( fileContents ),
                tags = GetTags( fileContents )
            };
            return post;
        }

        private string GetString( byte[] bytes )
        {
            char[] chars = new char[bytes.Length / sizeof( char )];
            System.Buffer.BlockCopy( bytes, 0, chars, 0, bytes.Length );
            return new string( chars );
        }

        private string GetAuthor( string path )
        {
            var directories = path.Split( '\\' );
            var author = directories.ElementAt( directories.Length - 2 );
            author = author.Replace( "_", " " );
            author = Regex.Replace( author, @"\b(\w)", m => m.Value.ToUpper() );
            return author;
        }

        private DateTime GetDate( string path )
        {
            var directories = path.Split( '\\' );
            var exampleDate = DateTime.Now.ToString( "yyyy_MM_dd" );
            var date = directories.Last().Substring( 0, exampleDate.Length );
            return DateTime.Parse( date.Replace( "_", " " ) );
        }

        private string GetTitle( string path )
        {
            var directories = path.Split( '\\' );
            var fileNameWithoutExtension = directories.Last().Split( '.' ).First();
            var exampleDate = DateTime.Now.ToString( "yyyy_MM_dd" );
            var title = fileNameWithoutExtension.Substring( exampleDate.Length + 1 );
            return title.Replace( "_", " " );
        }

        private string GetBody( string fileContents )
        {
            var tags = fileContents.Split( new string[]{ Environment.NewLine }, StringSplitOptions.None ).First();
            var body = fileContents.Remove( 0, tags.Length + ( 2 * Environment.NewLine.Length ) );
            return body;
        }

        private string[] GetTags( string fileContents )
        {
            var tags = fileContents.Split( new string[] { Environment.NewLine }, StringSplitOptions.None ).First();
            var tagArray = tags.Split( ':' );
            return tagArray.Where( x => x != "" ).ToArray();
        }
    }
}
