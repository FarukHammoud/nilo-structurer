namespace Domain {
    public interface ISchemeProvider {
        INumericalScheme GetScheme(IProcessDynamics dynamics);
    }
}
