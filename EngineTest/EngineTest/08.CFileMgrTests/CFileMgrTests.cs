using Engine._08.CFileMgr;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EngineTest._08.CFileMgrTest
{
    public class CFileMgrTests
    {
        private MockRepository mockRepository;


        public CFileMgrTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
        }

        private CFileMgr CreateCFileMgr()
        {
            //return new CFileMgr();
            return CFileMgr.GetInstance();
        }
        [Fact]
        public void GetFileNames_ShouldReturnFileNames_WhenFilesSelected()
        {
            // Arrange
            var fileMgr = CreateCFileMgr();

            // Act
            var result = fileMgr.GetFileNames();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetFileNames_ShouldReturnNull_WhenNoFilesSelected()
        {
            // Arrange
            var fileMgr = CreateCFileMgr();

            // Act
            var result = fileMgr.GetFileNames();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetFileNames_WithPath_ShouldReturnFileNames()
        {
            // Arrange
            var fileMgr = CreateCFileMgr();
            var testDir = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
            Directory.CreateDirectory(testDir);
            File.WriteAllText(Path.Combine(testDir, "test.txt"), "test content");

            // Act
            var result = fileMgr.GetFileNames(testDir);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains("test.txt", result[0]);

            // Cleanup
            Directory.Delete(testDir, true);
        }

        [Fact]
        public void GetRecursiveFileNames_ShouldReturnDirectoriesWithFiles()
        {
            // Arrange
            var fileMgr = CreateCFileMgr();
            var testDir = Path.Combine(Directory.GetCurrentDirectory(), "TestFiles");
            var subDir = Path.Combine(testDir, "SubDir");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "test.txt"), "test content");

            // Act
            var result = fileMgr.GetRecursiveFileNames(testDir);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains(subDir, result[0]);

            // Cleanup
            Directory.Delete(testDir, true);
        }
    }
}
