namespace Domain {
    public record Currency(String Code) {
        public required String Name { get; set; }
    }
}
