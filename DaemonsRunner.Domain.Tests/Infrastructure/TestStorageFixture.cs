namespace DaemonsRunner.Domain.Tests.Infrastructure
{
    public class TestStorageFixture : IDisposable
    {
        private bool _disposed;

        private object _lock = new object();

        private readonly string _testDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Test");

        private readonly List<string> _testFilesNames = new();

        public string TestDirectoryPath => _testDirectoryPath;

        public TestStorageFixture()
        {
            lock(_lock)
            {
				var directoryInfo = new DirectoryInfo(TestDirectoryPath);
				if (!directoryInfo.Exists)
				{
					directoryInfo.Create();
				}

				for (int i = 0; i < 10; i++)
				{
					var fileInfo = new FileInfo(Path.Combine(TestDirectoryPath, $"file{i}.php"));
					using var stream = fileInfo.Create();
					_testFilesNames.Add(fileInfo.Name);
				}
			}
        }

        public string GetRandomFileName() => _testFilesNames[new Random().Next(_testFilesNames.Count)];

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            lock(_lock )
            {
				var dirInfo = new DirectoryInfo(TestDirectoryPath);
				var files = dirInfo.GetFiles();
				foreach (var file in files)
				{
					file.Delete();
				}
				_disposed = true;
			}
        }
    }
}
