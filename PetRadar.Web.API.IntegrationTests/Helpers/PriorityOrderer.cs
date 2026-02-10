using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit;
using Xunit.Abstractions;

namespace PetRadar.Web.API.IntegrationTests.Helpers
{
    public class PriorityOrderer : ITestCaseOrderer
    {
        public const string PriorityOrdererName = "PetRadar.Web.API.IntegrationTests.Helpers.PriorityOrderer";
        public const string PriorityOrdererAssemblyName = "PetRadar.Web.API.IntegrationTests";

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                int priority = 0;
                foreach (var attr in testCase.TestMethod.Method.GetCustomAttributes(typeof(TestPriority).AssemblyQualifiedName))
                    priority = attr.GetNamedArgument<int>(nameof(TestPriority.Priority));

                GetOrCreate(sortedMethods, priority).Add(testCase);
            }

            foreach (var item in sortedMethods)
            {
                foreach (var testCase in item.Value)
                    yield return testCase;
            }
        }

        static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            if (dictionary.TryGetValue(key, out TValue result)) return result;

            result = new TValue();
            dictionary[key] = result;

            return result;
        }
    }
}
