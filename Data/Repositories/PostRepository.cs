using System;
using System.Collections.Generic;
using Application.RepositoryContracts;
using Application.Entities;
using System.IO;

namespace Data.Repositories
{
    public class PostRepository : IPostRepository
    {
        private string rootDirectory;

        public void CreatePost( string title, string body, DateTime date, string author )
        {
            CreateAuthorDirectory(author);
            var fullFilePath = string.Format( "{0}/{1}/{2}",
                rootDirectory, AuthorDirectory( author ), FileName( date, title ) );
            WriteToFile( fullFilePath, body );
        }

        public void WriteToFile( string fullFilePath, string body )
        {
            using ( var fileStream = File.Create( fullFilePath ) )
            {
                var fileBody = GetBytes( body );
                foreach( byte b in fileBody )
                    fileStream.WriteByte( b );
            }
        }

        public void SetRootDirectory( string directory )
        {
            rootDirectory = directory;
        }

        public IEnumerable<Post> GetAllPosts()
        {
            return new Post[1];
        }

        public void CreateAuthorDirectory( string author )
        {
            var directory = string.Format( "{0}/{1}", rootDirectory, AuthorDirectory( author ) );
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

        private byte[] GetBytes( string str )
        {
            byte[] bytes = new byte[str.Length * sizeof( char )];
            System.Buffer.BlockCopy( str.ToCharArray(), 0, bytes, 0, bytes.Length );
            return bytes;
        }
    }
}
