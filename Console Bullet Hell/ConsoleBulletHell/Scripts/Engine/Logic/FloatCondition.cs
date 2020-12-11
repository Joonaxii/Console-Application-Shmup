namespace Joonaxii.ConsoleBulletHell
{
    public class FloatCondition<TT> : Condition<float, TT> where TT : Entity
    {
        public override bool IsTrue
        {
            get
            {
                if (CompareToProperty())
                {
                    return _compareAgainst.CompareTo(GetCompareValue(), _compType);
                }

                return  GetValue().CompareTo(_compareToProperty ? GetCompareValue() : _compareAgainst, _compType);
            }
        }

        private ComparisonType _compType;

        public FloatCondition(ComparisonType comparisonType, string propName, float compareAgainst) : base(propName, compareAgainst)
        {
            _compType = comparisonType;
        }

        public FloatCondition(ComparisonType comparisonType, string propName, string compareAgainst) : base(propName, compareAgainst)
        {
            _compType = comparisonType;
        }

        public FloatCondition(ComparisonType comparisonType, float value, string compareAgainst) : base(value, compareAgainst)
        {
            _compType = comparisonType;
        }
    }
}