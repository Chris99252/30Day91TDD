using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestSample;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Microsoft.QualityTools.Testing.Fakes;
namespace UnitTestSample.Tests
{    

    [TestClass()]
    public class PubTests
    {
        #region Stub

        // Stub 通常使用在驗證目標回傳值，以及驗證目標物件狀態的改變。
        // 透過 stub object 直接模擬外部相依物件回傳的資料，來驗證目標物件行為是否如同預期

        #region 驗證目標回傳值
        [TestMethod()]
        public void Test_Charge_Customer_Count()
        {
            //arrange

            // 透過 MockRepository.GenerateStub<T>()
            // 來建立某一個 T 型別的 stub object
            // 以例子來說，是建立 ICheckInFee 介面的實作子類。
            ICheckInFee stubCheckInFee = MockRepository.GenerateStub<ICheckInFee>();

            // 把該 stub object 透過建構式，設定給測試目標物件。
            Pub target = new Pub(stubCheckInFee);

            // 定義當呼叫到該 stub object 的哪一個方法時，若傳入的參數為何，則 stub 要回傳什麼。
            stubCheckInFee.Stub(x => x.GetFee(Arg<Customer>.Is.Anything)).Return(100);

            var customers = new List<Customer>
            {
                new Customer{ IsMale=true},
                new Customer{ IsMale=false},
                new Customer{ IsMale=false},
            };

            decimal expected = 1;

            //act
            var actual = target.CheckInFriday(customers);

            //assert
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region 驗證目標物件狀態的改變
        [TestMethod]
        public void Test_Income()
        {
            //arrange
            ICheckInFee stubCheckInFee = MockRepository.GenerateStub<ICheckInFee>();
            Pub target = new Pub(stubCheckInFee);

            stubCheckInFee.Stub(x => x.GetFee(Arg<Customer>.Is.Anything)).Return(100);

            var customers = new List<Customer>
            {
                new Customer{ IsMale=true},
                new Customer{ IsMale=false},
                new Customer{ IsMale=false},
            };

            var inComeBeforeCheckIn = target.GetInCome();
            Assert.AreEqual(0, inComeBeforeCheckIn);

            decimal expectedIncome = 100;

            //act
            var chargeCustomerCount = target.CheckInFriday(customers);

            var actualIncome = target.GetInCome();

            //assert
            Assert.AreEqual(expectedIncome, actualIncome);
        }
        #endregion

        #endregion

        #region Mock

        // 我們想驗證的是：在2男1女的測試案例中，是否只呼叫 ICheckInFee 介面兩次

        [TestMethod()]
        public void Test_CheckIn_Charge_Only_Male()
        {
            //arrange mock
            var customers = new List<Customer>();
        
            //2男1女
            var customer1 = new Customer { IsMale = true };
            var customer2 = new Customer { IsMale = true };
            var customer3 = new Customer { IsMale = false };
        
            customers.Add(customer1);
            customers.Add(customer2);
            customers.Add(customer3);
        
            MockRepository mock = new MockRepository();
            ICheckInFee stubCheckInFee = mock.StrictMock<ICheckInFee>();
        
            using (mock.Record())
            {
                //期望呼叫ICheckInFee的GetFee()次數為2次
                stubCheckInFee.GetFee(customer1);
        
                LastCall
                    .IgnoreArguments()
                    .Return((decimal)100)
                    .Repeat.Times(2);
            }
        
            using (mock.Playback())
            {
                var target = new Pub(stubCheckInFee);
        
                var count = target.CheckInFriday(customers);
            }
        }
        #endregion

        #region Fake
        // 當目標物件使用到靜態方法，或 .net framework 本身的物件
        // 甚至於針對一般直接相依的物件，我們都可以透過 fake object 的方式
        // 直接模擬相依物件的行為

        // 1. 因為這個例子建立的 fake object，是針對 System.DateTime
        //    所以在測試專案上，針對 System.dll 來新增 Fake 組件
        // 2. 看到增加了一個 Fakes 的 folder，其中會針對要 fake 的 dll，產生對應的程式碼
        //    以便我們進行攔截與改寫

        #region Friday
        [TestMethod]
        public void Test_Friday_Charge_Customer_Count()
        {
            // 在 using (ShimsContext.Create()){} 的範圍中，會使用 Fake 組件。
            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.TodayGet = () =>
                {
                    //2012/10/19為Friday
                    return new DateTime(2015, 08, 07);
                };

                //arrange
                ICheckInFee stubCheckInFee = MockRepository.GenerateStub<ICheckInFee>();
                Pub target = new Pub(stubCheckInFee);

                stubCheckInFee.Stub(x => x.GetFee(Arg<Customer>.Is.Anything)).Return(100);

                var customers = new List<Customer>
                {
                    new Customer{ IsMale=true},
                    new Customer{ IsMale=false},
                    new Customer{ IsMale=false},
                };

                decimal expected = 1;

                //act
                var actual = target.CheckInFriday(customers);

                //assert
                Assert.AreEqual(expected, actual);
            }
        }
        #endregion

        #region Saturday
        [TestMethod]
        public void Test_Saturday_Charge_Customer_Count()
        {

            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.TodayGet = () =>
                {
                    //2012/10/20為Saturday
                    return new DateTime(2015, 08, 08);
                };

                //arrange
                ICheckInFee stubCheckInFee = MockRepository.GenerateStub<ICheckInFee>();
                Pub target = new Pub(stubCheckInFee);

                stubCheckInFee.Stub(x => x.GetFee(Arg<Customer>.Is.Anything)).Return(100);

                var customers = new List<Customer>
                {
                    new Customer{ IsMale=true},
                    new Customer{ IsMale=false},
                    new Customer{ IsMale=false},
                };

                decimal expected = 3;

                //act
                var actual = target.CheckInFriday(customers);

                //assert
                Assert.AreEqual(expected, actual);
            }
        }
        #endregion

        #endregion
    }
}
