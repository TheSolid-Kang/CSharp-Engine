using Engine._01.DBMgr;
using Moq;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Xunit;
using System.Data;

namespace EngineTest._01.DbMgrTests
{
    public partial class _TCDiary
    {

        /// <summary>
        /// 교회내부코드
        /// </summary>
        public int ChurchSeq { get; set; }

        /// <summary>
        /// 일기내부코드
        /// </summary>
        [Key]
        public int DiarySeq { get; set; }

        /// <summary>
        /// 일자
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 일기제목
        /// </summary>
        [StringLength(256)]
        public string? Title { get; set; }

        /// <summary>
        /// 일기본문
        /// </summary>
        public string? Record { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        [StringLength(200)]
        public string? Remark { get; set; }

        /// <summary>
        /// 최종작업자
        /// </summary>
        public int? LastUserSeq { get; set; }

        /// <summary>
        /// 최종작업일시
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime? LastDateTime { get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return base.ToString();
        }
    }

    public class DbMgrTests
    {
        private MockRepository mockRepository;
        private string dbUrl = "Data Source = localhost; Initial Catalog = Caleb; Persist Security Info = True; User ID = sa; Password = qp06910691!";


        public DbMgrTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        private DbMgr CreateDbMgr()
        {
            return new DbMgr();
        }

        [Fact]
        public void SelectList_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var dbMgr = this.CreateDbMgr();
            string _url = dbUrl;
            string _query = "SELECT * FROM _TCDiary";

            // Act
            var result = dbMgr.SelectList<_TCDiary>(
                _url,
                _query);

            // Assert
            Assert.True(result?.Count > 0);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void ConvertListToDataTable_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var dbMgr = this.CreateDbMgr();
            string _url = dbUrl;
            string _query = "SELECT * FROM _TCDiary";
            List<_TCDiary> items = dbMgr.SelectList<_TCDiary>(
                _url,
                _query);

            // Act
            var result = dbMgr.ConvertListToDataTable<_TCDiary>(
                items);

            // Assert
            Assert.True(result?.Rows.Count > 0);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void ConvertDataTableToList_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var dbMgr = this.CreateDbMgr();
            string _url = dbUrl;
            string _query = "SELECT * FROM _TCDiary";
            List<_TCDiary> items = dbMgr.SelectList<_TCDiary>(
                _url,
                _query);

            // Act
            DataTable _dataTable = dbMgr.ConvertListToDataTable<_TCDiary>(
                items);


            // Act
            var result = dbMgr.ConvertDataTableToList<_TCDiary>(
                _dataTable);

            // Assert
            Assert.True(result.Count > 0);
            this.mockRepository.VerifyAll();
        }
    }
}
