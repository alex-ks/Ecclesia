using System;

namespace Ecclesia.Resolver.Storage.Models
{
    public class AtomId
    {
        public struct SortCriteriaValue : IComparable<SortCriteriaValue>
        {
            private int _major, _middle, _minor;

            public SortCriteriaValue(int major, int middle, int minor)
            {
                _major = major;
                _middle = middle;
                _minor = minor;
            }

            public int CompareTo(SortCriteriaValue other)
            {
                int majorDiff = this._major - other._major;
                int middleDiff = this._middle - other._middle;
                int minorDiff = this._minor - other._minor;
                if (majorDiff != 0)
                    return majorDiff;
                else if (middleDiff != 0)
                    return middleDiff;
                else
                    return minorDiff;
            }

            public (int, int, int) AsTuple() => (_major, _middle, _minor);
        }

        public const string DefaultVersion = VersionUtils.DefaultVersion;

        private int _majorVersion = int.MaxValue;
        private int? _middleVersion = int.MaxValue;
        private int? _minorVersion = int.MaxValue;
        private string _name = null;
        private string _version = null;

        public string Kind { get; set; }
        public string Name { get; set; }
        public string Version 
        {
            get => _version;
            set 
            {
                if (value == null)
                {
                    _majorVersion = int.MaxValue;
                    _middleVersion = _minorVersion = int.MaxValue;
                }
                else if (!VersionUtils.TryParse(value, 
                                                out _majorVersion, 
                                                out _middleVersion, 
                                                out _minorVersion, 
                                                out _name))
                    throw new FormatException();
                _version = value;
            }
        }

        public SortCriteriaValue SortCriteria()
        {
            return new SortCriteriaValue(_majorVersion, 
                                         _middleVersion.GetValueOrDefault(), 
                                         _minorVersion.GetValueOrDefault());
        }
    }
}