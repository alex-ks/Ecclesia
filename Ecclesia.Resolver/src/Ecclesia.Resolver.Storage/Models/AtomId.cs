using System;

namespace Ecclesia.Resolver.Storage.Models
{
    public class AtomId
    {
        private int _majorVersion = 1;
        private int? _middleVersion = 0;
        private int? _minorVersion = 0;
        private string _name = null;
        private string _version = VersionUtils.DefaultVersion;

        public string Kind { get; set; }
        public string Name { get; set; }
        public string Version 
        {
            get => _version;
            set 
            {
                if (!VersionUtils.TryParse(value, out _majorVersion, out _middleVersion, out _minorVersion, out _name))
                    throw new ArgumentException("Invalid version format");
                _version = value;
            }
        }

        public (int, int, int) SortCriteria()
        {
            return (_majorVersion, _middleVersion.GetValueOrDefault(), _minorVersion.GetValueOrDefault());
        }
    }
}