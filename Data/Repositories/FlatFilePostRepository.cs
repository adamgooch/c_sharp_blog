using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using Application.Posts.RepositoryContracts;
using Application.Posts.Entities;

namespace Data.Repositories
{
    public class FlatFilePostRepository : IPostRepository
    {
        private readonly string rootDirectory = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath +
            ConfigurationManager.AppSettings["PostRootDirectory"];

        public void CreatePost( Post post )
        {
            CreateAuthorDirectory( post.Author );
            var fullFilePath = string.Format( "{0}\\{1}\\{2}",
                rootDirectory, AuthorDirectory( post.Author ), FileName( post.Date, post.Title ) );
            WriteToFile( fullFilePath, post.Tags, post.Body );
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
            var datePath = date.ToString( "yyyy_MM_dd" );
            var titlePath = title.Replace( " ", "_" );
            var fileName = string.Format( "{0}_{1}.html", datePath, titlePath );
            return fileName;
        }

        private void WriteToFile( string fullFilePath, IEnumerable<string> tags, string body )
        {
            using( var fileStream = File.Create( fullFilePath ) )
            {
                WriteTags( fileStream, tags );
                WriteStringToFile( fileStream, body );
            }
        }

        private void WriteTags( FileStream fileStream, IEnumerable<string> tags )
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
            foreach( var b in GetBytes( text ) )
                fileStream.WriteByte( b );
        }

        private IEnumerable<byte> GetBytes( string str )
        {
            var bytes = new byte[str.Length * sizeof( char )];
            Buffer.BlockCopy( str.ToCharArray(), 0, bytes, 0, bytes.Length );
            return bytes;
        }

        public IEnumerable<Post> GetAllPosts( string author )
        {
            var path = string.Format( "{0}\\{1}", rootDirectory, AuthorDirectory( author ) );
            var allPostPaths = Directory.GetFiles( path );
            return allPostPaths.Select( MapFileToPost );
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return GetAllFiles().Select( MapFileToPost );
        }

        private IEnumerable<string> GetAllFiles()
        {
            if( !Directory.Exists( rootDirectory ) ) Directory.CreateDirectory( rootDirectory );
            return Directory.GetDirectories( rootDirectory ).SelectMany( Directory.GetFiles );
        }

        public Post MapFileToPost( string filePath )
        {
            var readBuffer = File.ReadAllBytes( filePath );
            var fileContents = GetString( readBuffer );
            var post = new Post
            {
                Author = GetAuthor( filePath ),
                Date = GetDate( filePath ),
                Title = GetTitle( filePath ),
                Body = GetBody( fileContents ),
                Tags = GetTags( fileContents )
            };
            return post;
        }

        private string GetString( byte[] bytes )
        {
            var chars = new char[bytes.Length / sizeof( char )];
            Buffer.BlockCopy( bytes, 0, chars, 0, bytes.Length );
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
            var fileName = directories.Last();
            var fileNameWithoutExtension = fileName.Remove( fileName.Length - ".html".Length );
            var exampleDate = DateTime.Now.ToString( "yyyy_MM_dd" );
            var title = fileNameWithoutExtension.Substring( exampleDate.Length + 1 );
            return title.Replace( "_", " " );
        }

        private string GetBody( string fileContents )
        {
            var tags = fileContents.Split( new string[] { Environment.NewLine }, StringSplitOptions.None ).First();
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
