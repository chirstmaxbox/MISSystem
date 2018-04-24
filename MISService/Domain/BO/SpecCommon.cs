using System.Collections.Generic;
using MyCommon;

namespace SpecDomain.BO
{
    public class SpecCommon
    {
        public static string GetEmployeeUserName(string userIdentityName)
        {
            char[] splitter = {'\\'};
            string[] serverPrefix = userIdentityName.Split(splitter);
            return serverPrefix[1].ToUpper();

        }


        public static List<MyKeyValuePair> GetDispatchingTaskStatus()
        {
            var k1 = new MyKeyValuePair() {Key = 201, Value = "New"};
            var k2 = new MyKeyValuePair() {Key = 211, Value = "Working"};
            var k3 = new MyKeyValuePair() {Key = 215, Value = "Outsourcing"};
            var k4 = new MyKeyValuePair() {Key = 221, Value = "Problem"};
            var k5 = new MyKeyValuePair() {Key = 231, Value = "Revise"};
            var k6 = new MyKeyValuePair() {Key = 2491, Value = "Finish"};
            return new List<MyKeyValuePair>() {k1, k2, k3, k4, k5, k6};

        }
    }
}