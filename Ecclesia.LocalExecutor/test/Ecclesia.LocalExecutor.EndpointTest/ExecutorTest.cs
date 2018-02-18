using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.IO;
using System.Threading;
using System.Linq;
using Ecclesia.LocalExecutor.Endpoint;
using System.Threading.Tasks;

namespace Ecclesia.LocalExecutor.EndpointTest
{
    public class ExecutorTest
    {
        private Executor _executor;
        private const string scriptName = "script";

        class TestMethodManager : IMethodManager
        {
            private string script = File.ReadAllText("testMethod.py");

            public string GetMethodSource(string methodName)
            {
                return script;
            }
        }

        public ExecutorTest()
        {
            _executor = new Executor(new TestMethodManager());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(10)]
        [InlineData(42)]
        public void DoubleValues_SingleInputSingleExecution_SingleOutput(int input)
        {
            string[] outputs = null;

            _executor.Add(scriptName, new [] { input.ToString() }, os => outputs = os).Wait();

            Assert.NotNull(outputs);
            Assert.Single(outputs);
            
            Assert.True(int.TryParse(outputs[0], out int result));
            Assert.Equal(input * 2, result);
        }

        [Fact]
        public void DoubleValues_SingleInputMultipleExecutions_SingleOutput()
        {
            var inputs = Enumerable.Range(0, 10).ToArray();
            var outputs = new string[inputs.Length] [];

            var tasks = inputs
                .Select(i => _executor.Add(scriptName, new [] { i.ToString() }, os => outputs[i] = os))
                .ToArray();

            Task.WaitAll(tasks);

            Assert.All(outputs, output => 
            {
                Assert.NotNull(output);
                Assert.Single(output);
            });

            var pairs = Enumerable.Zip(inputs, outputs, (input, output) => (input, output[0]));

            Assert.All(pairs, pair => 
            {
                var (input, output) = pair;
                Assert.True(int.TryParse(output, out int result));
                Assert.Equal(input * 2, result);
            });
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 2, 3, 4)]
        [InlineData(3, 7, 42, 256, 1024)]
        public void DoubleValues_SingleTaskMultipleInputs_MultipleOutputs(params int[] inputs)
        {
            string[] outputs = null;
            _executor.Add(scriptName, inputs.Select(i => i.ToString()).ToArray(), os => outputs = os).Wait();
            
            Assert.NotNull(outputs);
            Assert.Equal(inputs.Length, outputs.Length);
            
            var pairs = Enumerable.Zip(inputs, outputs, (input, output) => (input, output));

            Assert.All(pairs, pair => 
            {
                Assert.True(int.TryParse(pair.output, out int result));
                Assert.Equal(pair.input * 2, result);
            });
        }
    }
}
