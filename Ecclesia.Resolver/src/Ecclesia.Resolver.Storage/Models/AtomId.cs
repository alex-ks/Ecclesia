using System;

namespace Ecclesia.Resolver.Storage.Models
{
    public struct AtomId : IComparable<AtomId>
    {
        public const string DefaultVersion = VersionUtils.DefaultVersion;

        private int _majorVersion;
        private int? _middleVersion;
        private int? _minorVersion;
        private string _versionName;

        public string Kind { get; }
        public string Name { get; }
        public string Version { get; }

        public AtomId(string kind, string name) : this(kind, name, null) { }

        public AtomId(string kind, string name, string version)
        {
            if (kind == null || name == null)
                throw new ArgumentException("Name and kind cannot be null");

            Kind = kind;
            Name = name;

            if (version == null)
            {
                _majorVersion = int.MaxValue;
                _middleVersion = _minorVersion = int.MaxValue;
                _versionName = null;
            }
            else if (!VersionUtils.TryParse(version, 
                                            out _majorVersion, 
                                            out _middleVersion, 
                                            out _minorVersion, 
                                            out _versionName))
                throw new FormatException();
            Version = version;
        }

        public int CompareTo(AtomId other)
        {
            int majorDiff = this._majorVersion - other._majorVersion;
            int middleDiff = this._middleVersion.GetValueOrDefault() - other._middleVersion.GetValueOrDefault();
            int minorDiff = this._minorVersion.GetValueOrDefault() - other._minorVersion.GetValueOrDefault();
            if (majorDiff != 0)
                return majorDiff;
            else if (middleDiff != 0)
                return middleDiff;
            else
                return minorDiff;
        }

        public (int, int?, int?, string) VersionTuple => (_majorVersion, _middleVersion, _minorVersion, _versionName);

        public override int GetHashCode()
        {
            return Kind.GetHashCode() ^ Name.GetHashCode() ^ (Version?.GetHashCode() ?? 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AtomId))
                return false;
            var another = (AtomId)obj;
            return this.Kind == another.Kind
                   && this.Name == another.Name
                   && this.Version == another.Version;
        }
    }
}