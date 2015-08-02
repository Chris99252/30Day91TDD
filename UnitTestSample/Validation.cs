using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestSample
{
    /// <summary>
    /// 為了取得密碼，而直接初始化 AccountDao 物件
    /// 為了取得 hash 結果，而直接初始化 Hash 物件
    /// 所以 Validation 物件便與 AccountDao 物件以及 Hash 物件直接相依
    /// 
    /// 如何隔離物件之間的相依性呢?

    /// 方法一：相依於介面
    ///   1. 建構式
    ///   2. 公開屬性（private get; public set）
    ///   3. 呼叫方法時傳入參數
    ///   
    /// 方法二：繼承使用可覆寫的保護方法  
    /// </summary>

    #region 物件存在相依性
    #if Release
    public class Validation
    {
        public bool CheckAuthentication(string id, string password)
        {
            // 取得資料中，id所對應的密碼
            AccountDao dao = new AccountDao();
            var passwordByDao = dao.GetPassword(id);

            // 針對傳入的password，進行hash運算
            Hash hash = new Hash();
            var hashResult = hash.GetHashResult(password);

            // 比對hash後的密碼，與資料中的密碼是否吻合
            return passwordByDao == hashResult;
        }
    }
    public class AccountDao
    {
        internal object GetPassword(string id)
        {
            throw new NotImplementedException();
        }
    }
    public class Hash
    {
        internal object GetHashResult(string password)
        {
            throw new NotImplementedException();
        }
    }
    #endif
    #endregion

    #region 消除相依性 - 相依於介面

    public interface IAccountDao
    {
        string GetPassword(string id);
    }

    public interface IHash
    {
        string GetHashResult(string password);
    }

    public class AccountDao : IAccountDao
    {
        public string GetPassword(string id)
        {
            throw new NotImplementedException();
        }
    }

    public class Hash : IHash
    {
        public string GetHashResult(string password)
        {
            throw new NotImplementedException();
        }
    }

    public class StubHash : IHash 
    {
        public string GetHashResult(string password)
        {
            return "Chris";
        }
    }

    public class StubAccountDao : IAccountDao
    {
        public string GetPassword(string id)
        {
            return "Chris";
        }
    }

    public class Validation
    {
        #region 公開屬性（public setter property）

        // 公開屬性通常只會將 setter 公開給外部設定，getter 則設定為 private
        // 原因很簡單，外部只需設定，而不需取用

        public IAccountDao _accountDao { private get; set; }

        public IHash _hash { private get; set; }
        #endregion        

        #region 建構式（constructor）

        // 將物件的相依介面，拉到公開的建構式，供外部物件使用時

        //private IAccountDao _accountDao;
        //private IHash _hash;

        public Validation(IAccountDao dao, IHash hash)
        {
            this._accountDao = dao;
            this._hash = hash;
        }
        #endregion

        public bool CheckAuthentication(string id, string password)
        {
            if (this._accountDao == null)
            {
                throw new ArgumentNullException();
            }

            if (this._hash == null)
            {
                throw new ArgumentNullException();
            }

            // 取得資料中，id對應的密碼                       
            var passwordByDao = this._accountDao.GetPassword(id);

            // 針對傳入的password，進行hash運算
            var hashResult = this._hash.GetHashResult(password);

            // 比對hash後的密碼，與資料中的密碼是否吻合
            return passwordByDao == hashResult;
        }

        #region 呼叫方法時傳入參數（function parameter）
        #if Release
        public bool CheckAuthentication(IAccountDao accountDao, IHash hash, string id, string password)
        {
            var passwordByDao = accountDao.GetPassword(id);
            var hashResult = hash.GetHashResult(password);

            return passwordByDao == hashResult;
        }
        #endif
        #endregion
    }
    #endregion

    #region 消除相依性 - 繼承可覆寫的保護方法（protected virtual function）
    public class ValidationPVF
    {
        // 1. 把 new 物件的動作抽離高層抽象的 context 中

        public bool CheckAuthenticationPVF(string id, string password)
        {
            var accountDao = GetAccountDao();
            var passwordByDao = accountDao.GetPassword(id);

            var hash = GetHash();
            var hashResult = hash.GetHashResult(password);

            return passwordByDao == hashResult;
        }

        // 2. 將兩個 new 物件的方法，宣告為 protected virtual，代表子類別可以繼承與覆寫該方法。

        protected virtual HashPVF GetHash()
        {
            var hash = new HashPVF();
            return hash;
        }

        protected virtual AccountDaoPVF GetAccountDao()
        {
            var accountDao = new AccountDaoPVF();
            return accountDao;
        }
    }

    // 3. 將要使用到 HashPVF 與 AccountDaoPVF 的方法，也要宣告為 virtual

    public class AccountDaoPVF
    {
        public virtual string GetPassword(string id)
        {
            throw new NotImplementedException();
        }
    }

    public class HashPVF
    {
        public virtual string GetHashResult(string password)
        {
            throw new NotImplementedException();
        }
    }

    // 4. StubAccountDaoPVF 改為繼承自 AccountDaoPVF，並將原本 public 的方法，加上 override 關鍵字，覆寫其父類方法內容。

    public class StubHashPVF : HashPVF
    {
        public override string GetHashResult(string password)
        {
            return "Chris";
        }
    }

    public class StubAccountDaoPVF : AccountDaoPVF
    {
        public override string GetPassword(string id)
        {
            return "Chris";
        }
    }

    // 5. 建立一個 MyValidation 的 class，繼承自 ValidationPVF。
    //    並覆寫 GetAccountDao() 與 GetHash()，使其回傳 Stub Object。

    public class MyValidation : ValidationPVF
    {
        protected override AccountDaoPVF GetAccountDao()
        {
            return new StubAccountDaoPVF();
        }

        protected override HashPVF GetHash()
        {
            return new StubHashPVF();
        }
    }
    #endregion
}
