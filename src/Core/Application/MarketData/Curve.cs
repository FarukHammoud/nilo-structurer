namespace Application {
    public class Curve {
        public DateTime? FirstDate => _values.Count > 0 ? _values.GetAt(0).Key : null;
        public DateTime? LastDate => _values.Count > 0 ? _values.GetAt(_values.Count - 1).Key : null;
        
        private OrderedDictionary<DateTime, double> _values;

        public Curve() {
            _values = new OrderedDictionary<DateTime, double>();
        }

        public Curve(OrderedDictionary<DateTime, double> values) {
            _values = values;
        }

        public Curve setNode(DateTime date, double value) {
            _values[date] = value;
            return this;
        }

        public double GetValue(DateTime date) {
            if (_values.ContainsKey(date)) {
                return (double)_values[date];
            }
            if (_values.Count > 0 && date < _values.GetAt(0).Key) {
                return (double)_values[_values.GetAt(0).Key];
            }
            if (_values.Count > 0 && date > _values.GetAt(_values.Count - 1).Key) {
                return (double)_values[_values.GetAt(_values.Count - 1).Key];
            } 

            DateTime keyMin = DateTime.MinValue;
            DateTime keyMax = DateTime.MaxValue;
            foreach (DateTime key in _values.Keys) {
                if (key < date) {
                    keyMin = key;
                } else {
                    keyMax = key;
                    break;
                }
            }
            return _values[keyMin] + (_values[keyMax] - _values[keyMin]) * (date - keyMin).TotalDays / (keyMax - keyMin).TotalDays;           
        }

        public OrderedDictionary<DateTime, double> GetNodes() {
            return _values;
        }
    }
}
