namespace Domain {
    public interface IJumpProcess {
        // return a sample from a jump process,
        // for a time increase dt and
        // a 0-1 uniformally distributed sample u
        double Sample(double dt, double u);
    }
}
