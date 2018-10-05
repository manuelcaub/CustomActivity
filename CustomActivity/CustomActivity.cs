using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace CustomActivity
{
    public class Should
    {
        [Fact]
        public async Task GetSameActivity()
        {
            using (CustomActivity.Start())
            {
                var somethingAsync = await Something.Service.GetAsync();
                var something = new Something.Service().Get();

                Assert.Equal(somethingAsync, something);
            }
        }
    }

    internal class Something
    {
        public class Service
        {
            public string Get()
            {
                return $"{Activity.Current.Id} /// {Activity.Current.RootId} /// {Activity.Current.ParentId}";
            }

            public static async Task<string> GetAsync()
            {
                return await Task.FromResult($"{Activity.Current.Id} /// {Activity.Current.RootId} /// {Activity.Current.ParentId}");
            }
        }
    }

    internal class CustomActivity : IDisposable
    {
        private readonly Activity activity;
        private readonly DiagnosticSource diagnosticSource;

        private CustomActivity(string listener, string operation)
        {
            diagnosticSource = new DiagnosticListener(listener);
            activity = new Activity(operation);
        }

        public static CustomActivity Start()
        {
            var customActivity = new CustomActivity(nameof(CustomActivity), nameof(CustomActivity));
            customActivity.diagnosticSource.StartActivity(customActivity.activity, null);
            return customActivity;
        }

        public void Dispose()
        {
            diagnosticSource.StopActivity(activity, null);
        }
    }
}
