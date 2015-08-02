using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestSample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTestSample.Tests
{
    /// <summary>
    /// 當呼叫 Validation 物件的 CheckAuthentication 方法時
    /// 就肯定會使用 AccountDao 的 GetPassword 方法
    /// 進而連線至 DB，取得對應的密碼資料
    /// 
    /// # 單元測試的定義與原則：單元測試必須與外部環境、類別、資源、服務獨立，而不能直接相依。
    /// 
    /// 透過 stub 物件來模擬，以驗證 Validation 物件本身的 CheckAuthentication 方法邏輯，是否符合預期。
    /// </summary>

    #region Test：物件存在相依性
    #if Release
    [TestClass()]
    public class ValidationTests
    {
        [TestMethod()]
        public void CheckAuthenticationTest()
        {
            Validation target = new Validation();
            string id = string.Empty; // TODO: 初始化為適當值
            string password = string.Empty; // TODO: 初始化為適當值
            bool expected = false; // TODO: 初始化為適當值
            bool actual;
            actual = target.CheckAuthentication(id, password);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("驗證這個測試方法的正確性。");
        }
    }
    #endif
    #endregion

    #region Test：利用介面，測試程式使用 Stub 物件，解決相依性
    [TestClass()]
    public class ValidationTests
    {
        [TestMethod()]
        #region CheckAuthenticationTest：利用介面，測試程式使用 Stub 物件，解決相依性
        public void CheckAuthenticationTest()
        {
            // [arrange]

            // 初始化StubAccountDao，來當作IAccountDao的執行個體
            IAccountDao dao = new StubAccountDao();

            //初始化StubHash，來當作IHash的執行個體
            IHash hash = new StubHash();

            // 將自訂的兩個stub object，注入到目標物件中，也就是Validation物件
            Validation target = new Validation(dao, hash);

            string id = "id隨便啦";
            string password = password = "密碼也沒關係";

            // 期望為true，因為預期hash後的結果是"Chris"，而IAccountDao回來的結果也是"Chris"，所以為true
            bool expected = true;

            // [act]
            bool actual;
            actual = target.CheckAuthentication(id, password);

            // [assert]
            Assert.AreEqual(expected, actual);
        }
        #endregion

        [TestMethod()]
        #region CheckAuthenticationPVFTest：利用繼承，可覆寫的保護方法，解決相依性
        public void CheckAuthenticationPVFTest()
        {
            ValidationPVF target = new MyValidation();

            string id = "id隨便啦";
            string password = "密碼也沒關係";

            bool expected = true;

            bool actual;
            actual = target.CheckAuthenticationPVF(id, password);

            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
    #endregion
}
