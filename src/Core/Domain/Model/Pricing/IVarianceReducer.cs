namespace Domain {
    public interface IVarianceReducer {
        List<double> Adjust(IEnumerable<double> discountedPayoffs);
    }
}
