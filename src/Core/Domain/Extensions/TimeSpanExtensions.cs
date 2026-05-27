namespace Domain {
    public static class TimeSpanExtensions {
        extension(TimeSpan timeSpan) {
            public double TotalYears => timeSpan.TotalDays / 365.0;
        }
    }
}
