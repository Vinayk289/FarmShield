namespace FarmShield.Models.Enums
{
    public static class AppRoles
    {
        public const string SuperAdmin = "Super Admin";
        public const string FarmOwner = "Farm Owner";
        public const string FarmManager = "Farm Manager";
        public const string VeterinaryOfficer = "Veterinary Officer";

        public static readonly string[] AllRoles = [SuperAdmin, FarmOwner, FarmManager, VeterinaryOfficer];
    }

    public enum FarmType
    {
        Pig = 1,
        Poultry = 2,
        Mixed = 3
    }

    public enum FarmStatus
    {
        Active = 1,
        UnderAudit = 2,
        Quarantine = 3,
        Inactive = 4
    }

    public enum BioSafetyLevel
    {
        Level1_Standard = 1,
        Level2_Enhanced = 2,
        Level3_Maximum = 3
    }

    public enum PigCategory
    {
        Piglet = 1,
        Grower = 2,
        Sow = 3,
        Boar = 4
    }

    public enum AnimalGender
    {
        Male = 1,
        Female = 2
    }

    public enum AnimalHealthStatus
    {
        Healthy = 1,
        Sick = 2,
        Quarantine = 3,
        UnderTreatment = 4
    }

    public enum AnimalStatus
    {
        Active = 1,
        Sold = 2,
        Culled = 3,
        Deceased = 4
    }

    public enum PoultryType
    {
        Broiler = 1,
        Layer = 2,
        Chicks = 3
    }

    public enum PoultryBatchStatus
    {
        Active = 1,
        Completed = 2,
        Depleted = 3
    }

    public enum BiosecurityCategory
    {
        VisitorManagement = 1,
        CleaningDisinfection = 2,
        AnimalHealthMonitoring = 3,
        FeedWaterManagement = 4,
        PestRodentControl = 5,
        WasteManagement = 6
    }

    public enum ComplianceStatus
    {
        Compliant = 1,
        NonCompliant = 2,
        NeedsReview = 3,
        NotApplicable = 4
    }

    public enum ApprovalStatus
    {
        Pending = 1,
        Approved = 2,
        Denied = 3
    }

    public enum RiskLevel
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum InspectionStatus
    {
        Scheduled = 1,
        InProgress = 2,
        Completed = 3,
        FollowUpRequired = 4
    }

    public enum DiseaseSeverity
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum DiseaseVerificationStatus
    {
        Suspected = 1,
        UnderInvestigation = 2,
        Confirmed = 3,
        Resolved = 4
    }

    public enum VaccinationStatus
    {
        Scheduled = 1,
        Completed = 2,
        Overdue = 3,
        Cancelled = 4
    }

    public enum NotificationSeverity
    {
        Info = 1,
        Success = 2,
        Warning = 3,
        Danger = 4
    }

    public enum NotificationCategory
    {
        DiseaseAlert = 1,
        InspectionDue = 2,
        OverdueVaccination = 3,
        BiosecurityViolation = 4,
        CorrectiveAction = 5,
        System = 6
    }
}

