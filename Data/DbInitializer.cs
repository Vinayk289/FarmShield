using FarmShield.Models.Entities;
using FarmShield.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FarmShield.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure Database Created
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Roles
            foreach (var role in AppRoles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Default Users
            var superAdmin = await userManager.FindByEmailAsync("admin@farmshield.com");
            if (superAdmin == null)
            {
                superAdmin = new ApplicationUser
                {
                    UserName = "admin@farmshield.com",
                    Email = "admin@farmshield.com",
                    FullName = "Dr. Alexander Wright",
                    RoleName = AppRoles.SuperAdmin,
                    Designation = "Chief Biosecurity Inspector & System Director",
                    Specialization = "Epidemiological Surveillance & Veterinary Standards",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-12)
                };
                await userManager.CreateAsync(superAdmin, "FarmShield@2025");
                await userManager.AddToRoleAsync(superAdmin, AppRoles.SuperAdmin);
            }

            var farmOwner = await userManager.FindByEmailAsync("owner@greenvalleyfarms.com");
            if (farmOwner == null)
            {
                farmOwner = new ApplicationUser
                {
                    UserName = "owner@greenvalleyfarms.com",
                    Email = "owner@greenvalleyfarms.com",
                    FullName = "Marcus Sterling",
                    RoleName = AppRoles.FarmOwner,
                    Designation = "Managing Director & Principal Herd Owner",
                    Specialization = "Commercial Swine & Poultry Production",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-10)
                };
                await userManager.CreateAsync(farmOwner, "FarmShield@2025");
                await userManager.AddToRoleAsync(farmOwner, AppRoles.FarmOwner);
            }

            var farmManager = await userManager.FindByEmailAsync("manager@highlandpigs.com");
            if (farmManager == null)
            {
                farmManager = new ApplicationUser
                {
                    UserName = "manager@highlandpigs.com",
                    Email = "manager@highlandpigs.com",
                    FullName = "Sarah Jenkins",
                    RoleName = AppRoles.FarmManager,
                    Designation = "Biosecurity Compliance & Production Manager",
                    Specialization = "Intensive Herd Biosecurity Protocols",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-8)
                };
                await userManager.CreateAsync(farmManager, "FarmShield@2025");
                await userManager.AddToRoleAsync(farmManager, AppRoles.FarmManager);
            }

            var vetOfficer = await userManager.FindByEmailAsync("vet@biosecurevet.com");
            if (vetOfficer == null)
            {
                vetOfficer = new ApplicationUser
                {
                    UserName = "vet@biosecurevet.com",
                    Email = "vet@biosecurevet.com",
                    FullName = "Dr. Elena Rostova",
                    RoleName = AppRoles.VeterinaryOfficer,
                    Designation = "Senior Veterinary Field Officer",
                    Specialization = "Swine & Avian Pathology / Containment",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddMonths(-6)
                };
                await userManager.CreateAsync(vetOfficer, "FarmShield@2025");
                await userManager.AddToRoleAsync(vetOfficer, AppRoles.VeterinaryOfficer);
            }

            // 3. Seed Inspection Checklist Items
            if (!await context.InspectionChecklistItems.AnyAsync())
            {
                var checklistItems = new List<InspectionChecklistItem>
                {
                    new() { Category = "Perimeter Security", ItemTitle = "Double Security Perimeter Fencing", Description = "Farm perimeter is secured with intact 2-meter chain link fence and razor wire, free from vegetation and burrowing gaps.", StandardReference = "WOAH Biosecurity Code 4.3", Weight = 3, SortOrder = 1 },
                    new() { Category = "Perimeter Security", ItemTitle = "Locked Access Gates & Signage", Description = "All outer boundary gates remain locked 24/7 with visible biosecurity warning signs and visitor check-in instructions.", StandardReference = "FAO Animal Health Manual 18", Weight = 2, SortOrder = 2 },
                    new() { Category = "Visitor Control", ItemTitle = "Mandatory Visitor Gate Log & 72-Hour Rule", Description = "All visitors register complete contact details and must certify no contact with other pig/poultry farms in past 72 hours.", StandardReference = "USDA APHIS Biosecurity Guidelines", Weight = 3, SortOrder = 3 },
                    new() { Category = "Visitor Control", ItemTitle = "Farm-Dedicated Protective Clothing (PPE)", Description = "Boots, disposable coveralls, hairnets, and nitrile gloves supplied and worn by all visitors entering animal zones.", StandardReference = "ISO 22000 Feed & Farm Safety", Weight = 3, SortOrder = 4 },
                    new() { Category = "Entry Disinfection", ItemTitle = "Active Vehicle Tire Spray Station", Description = "Automated high-pressure disinfectant spray arch cleans tires, wheel wells, and chassis of all incoming feed and service trucks.", StandardReference = "OIE Terrestrial Animal Code", Weight = 3, SortOrder = 5 },
                    new() { Category = "Entry Disinfection", ItemTitle = "Footbath Maintenance & Concentration", Description = "Footbaths at all barn doors contain active virucidal disinfectant solution refreshed every 24-48 hours.", StandardReference = "DEFRA Biosecurity Standards", Weight = 3, SortOrder = 6 },
                    new() { Category = "Cleaning & Sanitation", ItemTitle = "All-In/All-Out Barn Sanitation Protocol", Description = "Thorough pressure washing, detergent foaming, and broad-spectrum disinfection conducted between successive animal batches.", StandardReference = "Swine Health Information Center", Weight = 3, SortOrder = 7 },
                    new() { Category = "Cleaning & Sanitation", ItemTitle = "Daily Equipment Disinfection Log", Description = "Feeders, drinkers, weigh scales, and syringes disinfected before reuse and logged with chemical batch numbers.", StandardReference = "EU Animal Health Regulation 2016/429", Weight = 2, SortOrder = 8 },
                    new() { Category = "Isolation & Quarantine", ItemTitle = "Dedicated Animal Quarantine Facility", Description = "Quarantine barn physically located at least 100 meters downwind from main barns with dedicated personnel.", StandardReference = "WOAH Terrestrial Code 4.4", Weight = 3, SortOrder = 9 },
                    new() { Category = "Isolation & Quarantine", ItemTitle = "30-Day New Stock Isolation Protocol", Description = "Newly introduced breeding pigs/replacement chicks held in isolation for minimum 30 days with serological screening.", StandardReference = "National Swine Disease Control", Weight = 3, SortOrder = 10 },
                    new() { Category = "Feed Safety", ItemTitle = "Moisture-Proof & Vermin-Sealed Silos", Description = "Bulk feed storage silos are tightly sealed, bird-proof, free of condensation, leaks, or mycotoxin/mold odors.", StandardReference = "FDA FSMA Animal Food Rule", Weight = 2, SortOrder = 11 },
                    new() { Category = "Feed Safety", ItemTitle = "Feed Delivery Driver Zoning", Description = "Feed delivery truck drivers remain strictly outside animal housing areas; feed discharged via external auger pipe.", StandardReference = "BioSecure Pork Standard 3.1", Weight = 2, SortOrder = 12 },
                    new() { Category = "Water Quality", ItemTitle = "Automated Water Chlorination / Sanitization", Description = "Drinking water continuously treated with 2-4 ppm free chlorine or approved organic acidifiers; recorded daily.", StandardReference = "WHO Livestock Drinking Standards", Weight = 3, SortOrder = 13 },
                    new() { Category = "Water Quality", ItemTitle = "Microbial Lab Water Testing", Description = "Monthly certified laboratory water analysis reports zero coliform bacteria and safe heavy metal/nitrate levels.", StandardReference = "EPA Agricultural Water Safety", Weight = 2, SortOrder = 14 },
                    new() { Category = "Pest Control", ItemTitle = "Mapped Rodent Bait Station Grid", Description = "Tamper-resistant bait stations spaced every 15 meters along building perimeters, inspected and replenished bi-weekly.", StandardReference = "IPM Biosecurity Protocol 7", Weight = 3, SortOrder = 15 },
                    new() { Category = "Pest Control", ItemTitle = "Fly & Wild Bird Exclusion Screens", Description = "Ventilation louvers and eaves fitted with 1-inch mesh screens to prevent wild bird entry and disease vectoring.", StandardReference = "FAO Poultry Biosecurity 2", Weight = 2, SortOrder = 16 },
                    new() { Category = "Waste Disposal", ItemTitle = "Biosecure Mortality Carcass Disposal", Description = "Dead animals disposed within 12 hours via high-temperature incinerator, sealed composting bay, or certified deep burial.", StandardReference = "State Environmental Protection Act", Weight = 3, SortOrder = 17 },
                    new() { Category = "Waste Disposal", ItemTitle = "Slurry Lagoon & Manure Buffer Storage", Description = "Slurry pits and lagoons lined with impervious HDPE membranes with minimum 60-day storage capacity away from water runoffs.", StandardReference = "Clean Water Ag Compliance", Weight = 2, SortOrder = 18 },
                    new() { Category = "Health & Vaccination", ItemTitle = "Complete Batch Vaccination Tracking", Description = "All compulsory immunizations (e.g., PRRS, PCV2, Swine Fever / Newcastle, Gumboro) administered and logged with lot numbers.", StandardReference = "WOAH Veterinary Vaccine Standards", Weight = 3, SortOrder = 19 },
                    new() { Category = "Health & Vaccination", ItemTitle = "Daily Morbidity & Mortality Threshold Alerts", Description = "Daily mortality logs monitored with immediate veterinary escalation triggered when mortality exceeds 0.5% in 24 hours.", StandardReference = "National Early Warning Animal Protocol", Weight = 3, SortOrder = 20 }
                };
                await context.InspectionChecklistItems.AddRangeAsync(checklistItems);
                await context.SaveChangesAsync();
            }

            // 4. Seed Farms
            if (!await context.Farms.AnyAsync())
            {
                var farm1 = new Farm
                {
                    FarmCode = "FS-PIG-001",
                    Name = "Green Valley Biosecure Pig Facility",
                    OwnerId = farmOwner.Id,
                    OwnerName = farmOwner.FullName,
                    FarmType = FarmType.Pig,
                    Address = "742 Prairie Bio-Corridor, West Pasture",
                    District = "Polk County",
                    State = "Iowa",
                    ContactNumber = "+1 (515) 892-4110",
                    Email = "operations@greenvalleypork.com",
                    FarmAreaAcres = 85.5,
                    RegistrationDate = DateTime.UtcNow.AddMonths(-14),
                    TotalCapacity = 3200,
                    Status = FarmStatus.Active,
                    BioSafetyLevel = BioSafetyLevel.Level3_Maximum,
                    Description = "State-of-the-art closed-herd farrow-to-finish swine facility implementing SPF (Specific Pathogen Free) protocols, filtered ventilation, and automated slurry digestion.",
                    ImageUrl = "https://images.unsplash.com/photo-1516467508483-a7212febe31a?auto=format&fit=crop&w=1200&q=80"
                };

                var farm2 = new Farm
                {
                    FarmCode = "FS-PLT-002",
                    Name = "Highland Broiler & Layer Agro-Complex",
                    OwnerId = farmOwner.Id,
                    OwnerName = farmOwner.FullName,
                    FarmType = FarmType.Poultry,
                    Address = "1208 Piedmont Ridge Highway",
                    District = "Hall County",
                    State = "Georgia",
                    ContactNumber = "+1 (770) 534-8890",
                    Email = "biosecure@highlandpoultry.com",
                    FarmAreaAcres = 120.0,
                    RegistrationDate = DateTime.UtcNow.AddMonths(-11),
                    TotalCapacity = 65000,
                    Status = FarmStatus.Active,
                    BioSafetyLevel = BioSafetyLevel.Level2_Enhanced,
                    Description = "Climate-controlled poultry production facility comprising 4 broiler houses and 2 layer barns with automated egg packing and strict biosecure perimeter buffer zones.",
                    ImageUrl = "https://images.unsplash.com/photo-1548550023-2bdb3c5beed7?auto=format&fit=crop&w=1200&q=80"
                };

                var farm3 = new Farm
                {
                    FarmCode = "FS-MXD-003",
                    Name = "Apex Integrated Agro-Livestock Research Farm",
                    OwnerId = farmOwner.Id,
                    OwnerName = farmOwner.FullName,
                    FarmType = FarmType.Mixed,
                    Address = "3340 Susquehanna Valley Road",
                    District = "Lancaster County",
                    State = "Pennsylvania",
                    ContactNumber = "+1 (717) 625-7711",
                    Email = "research@apexfarms.org",
                    FarmAreaAcres = 210.0,
                    RegistrationDate = DateTime.UtcNow.AddMonths(-8),
                    TotalCapacity = 18500,
                    Status = FarmStatus.Active,
                    BioSafetyLevel = BioSafetyLevel.Level3_Maximum,
                    Description = "Integrated multi-species agricultural research and breeding center demonstrating digital biosecurity compliance and early zoonotic pathogen surveillance.",
                    ImageUrl = "https://images.unsplash.com/photo-1500595046743-cd271d694d30?auto=format&fit=crop&w=1200&q=80"
                };

                await context.Farms.AddRangeAsync(farm1, farm2, farm3);
                await context.SaveChangesAsync();

                // Link managers to Farm 1
                farmManager.AssignedFarmId = farm1.FarmId;
                await userManager.UpdateAsync(farmManager);

                // 5. Seed Pigs for Farm 1 & 3
                var pigs = new List<Animal>
                {
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0101", Breed = "Large White Yorkshire", Category = PigCategory.Sow, Gender = AnimalGender.Female, DateOfBirth = DateTime.UtcNow.AddYears(-2), CurrentWeightKg = 220.5, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Breeding Barn A - Pen 4", Status = AnimalStatus.Active, RegistrationDate = DateTime.UtcNow.AddMonths(-12) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0102", Breed = "Landrace", Category = PigCategory.Sow, Gender = AnimalGender.Female, DateOfBirth = DateTime.UtcNow.AddYears(-2).AddMonths(3), CurrentWeightKg = 215.0, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Breeding Barn A - Pen 5", Status = AnimalStatus.Active, RegistrationDate = DateTime.UtcNow.AddMonths(-12) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0103", Breed = "Duroc", Category = PigCategory.Boar, Gender = AnimalGender.Male, DateOfBirth = DateTime.UtcNow.AddYears(-3), CurrentWeightKg = 295.0, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Boar Unit 1", Status = AnimalStatus.Active, RegistrationDate = DateTime.UtcNow.AddMonths(-14) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0104", Breed = "Hampshire", Category = PigCategory.Boar, Gender = AnimalGender.Male, DateOfBirth = DateTime.UtcNow.AddYears(-2).AddMonths(-4), CurrentWeightKg = 278.4, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Boar Unit 2", Status = AnimalStatus.Active, RegistrationDate = DateTime.UtcNow.AddMonths(-10) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0205", Breed = "Large White Cross", Category = PigCategory.Grower, Gender = AnimalGender.Male, DateOfBirth = DateTime.UtcNow.AddMonths(-5), CurrentWeightKg = 82.5, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Grower Barn B - Pen 12", Status = AnimalStatus.Active, MotherTag = "PG-0101", FatherTag = "PG-0103", RegistrationDate = DateTime.UtcNow.AddMonths(-5) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0206", Breed = "Large White Cross", Category = PigCategory.Grower, Gender = AnimalGender.Female, DateOfBirth = DateTime.UtcNow.AddMonths(-5), CurrentWeightKg = 79.2, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Grower Barn B - Pen 12", Status = AnimalStatus.Active, MotherTag = "PG-0101", FatherTag = "PG-0103", RegistrationDate = DateTime.UtcNow.AddMonths(-5) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0207", Breed = "Landrace Cross", Category = PigCategory.Grower, Gender = AnimalGender.Female, DateOfBirth = DateTime.UtcNow.AddMonths(-4), CurrentWeightKg = 64.0, HealthCondition = AnimalHealthStatus.UnderTreatment, PenBarnNumber = "Isolation Pen Iso-1", Status = AnimalStatus.Active, MotherTag = "PG-0102", FatherTag = "PG-0104", Remarks = "Mild coughing under veterinary antibiotic protocol.", RegistrationDate = DateTime.UtcNow.AddMonths(-4) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0308", Breed = "Large White Pure", Category = PigCategory.Piglet, Gender = AnimalGender.Female, DateOfBirth = DateTime.UtcNow.AddDays(-28), CurrentWeightKg = 8.4, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Farrowing House F-2", Status = AnimalStatus.Active, MotherTag = "PG-0101", FatherTag = "PG-0103", RegistrationDate = DateTime.UtcNow.AddDays(-28) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0309", Breed = "Large White Pure", Category = PigCategory.Piglet, Gender = AnimalGender.Male, DateOfBirth = DateTime.UtcNow.AddDays(-28), CurrentWeightKg = 9.1, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Farrowing House F-2", Status = AnimalStatus.Active, MotherTag = "PG-0101", FatherTag = "PG-0103", RegistrationDate = DateTime.UtcNow.AddDays(-28) },
                    new() { FarmId = farm1.FarmId, TagNumber = "PG-0310", Breed = "Landrace Pure", Category = PigCategory.Piglet, Gender = AnimalGender.Male, DateOfBirth = DateTime.UtcNow.AddDays(-20), CurrentWeightKg = 7.0, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Farrowing House F-3", Status = AnimalStatus.Active, MotherTag = "PG-0102", FatherTag = "PG-0104", RegistrationDate = DateTime.UtcNow.AddDays(-20) },

                    // Apex Mixed farm pigs
                    new() { FarmId = farm3.FarmId, TagNumber = "AP-001", Breed = "Berkshire Heritage", Category = PigCategory.Sow, Gender = AnimalGender.Female, DateOfBirth = DateTime.UtcNow.AddYears(-1).AddMonths(-6), CurrentWeightKg = 195.0, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Swine Research Barn S1", Status = AnimalStatus.Active, RegistrationDate = DateTime.UtcNow.AddMonths(-7) },
                    new() { FarmId = farm3.FarmId, TagNumber = "AP-002", Breed = "Berkshire Heritage", Category = PigCategory.Boar, Gender = AnimalGender.Male, DateOfBirth = DateTime.UtcNow.AddYears(-2), CurrentWeightKg = 240.0, HealthCondition = AnimalHealthStatus.Healthy, PenBarnNumber = "Swine Research Barn S1", Status = AnimalStatus.Active, RegistrationDate = DateTime.UtcNow.AddMonths(-7) }
                };
                await context.Animals.AddRangeAsync(pigs);
                await context.SaveChangesAsync();

                // 6. Seed Poultry Batches for Farm 2 & 3
                var batch1 = new PoultryBatch
                {
                    FarmId = farm2.FarmId,
                    BatchCode = "PB-2025-BR01",
                    BatchName = "Spring Broiler Batch Cobb-500 #1",
                    PoultryType = PoultryType.Broiler,
                    Breed = "Cobb 500 Fast Growth",
                    HatchDate = DateTime.UtcNow.AddDays(-34),
                    InitialBirdCount = 20000,
                    CurrentBirdCount = 19720,
                    HouseNumber = "Broiler Shed #1",
                    HousingType = "Environment Controlled Tunnel Ventilated",
                    Status = PoultryBatchStatus.Active,
                    TotalMortality = 280,
                    TargetFcr = 1.55,
                    CreatedAt = DateTime.UtcNow.AddDays(-34),
                    Remarks = "Standard brooder cycle completed with high flock uniformity."
                };

                var batch2 = new PoultryBatch
                {
                    FarmId = farm2.FarmId,
                    BatchCode = "PB-2025-LY02",
                    BatchName = "Commercial Layer Flock Hy-Line Brown",
                    PoultryType = PoultryType.Layer,
                    Breed = "Hy-Line Brown",
                    HatchDate = DateTime.UtcNow.AddDays(-140),
                    InitialBirdCount = 15000,
                    CurrentBirdCount = 14810,
                    HouseNumber = "Layer House #2",
                    HousingType = "Enriched Colony Aviary System",
                    Status = PoultryBatchStatus.Active,
                    TotalMortality = 190,
                    TargetFcr = 2.10,
                    CreatedAt = DateTime.UtcNow.AddDays(-140),
                    Remarks = "Peak lay period approaching with 94.2% daily production."
                };

                var batch3 = new PoultryBatch
                {
                    FarmId = farm3.FarmId,
                    BatchCode = "PB-2025-AP03",
                    BatchName = "Apex Trial Broiler Ross 308",
                    PoultryType = PoultryType.Broiler,
                    Breed = "Ross 308",
                    HatchDate = DateTime.UtcNow.AddDays(-18),
                    InitialBirdCount = 5000,
                    CurrentBirdCount = 4940,
                    HouseNumber = "Aviary Unit Alpha",
                    HousingType = "Deep Litter with Automated Misting",
                    Status = PoultryBatchStatus.Active,
                    TotalMortality = 60,
                    TargetFcr = 1.58,
                    CreatedAt = DateTime.UtcNow.AddDays(-18)
                };

                await context.PoultryBatches.AddRangeAsync(batch1, batch2, batch3);
                await context.SaveChangesAsync();

                // 7. Seed Poultry Daily Logs for Batch 1 and 2
                var dailyLogs = new List<PoultryDailyLog>
                {
                    new() { BatchId = batch1.BatchId, LogDate = DateTime.UtcNow.Date.AddDays(-4), MortalityCount = 6, FeedConsumedKg = 1850, WaterConsumedLiters = 3700, EggsCollected = 0, DamagedEggs = 0, AverageWeightGrams = 1750, Notes = "Flock active, water consumption normal.", RecordedBy = farmManager.FullName },
                    new() { BatchId = batch1.BatchId, LogDate = DateTime.UtcNow.Date.AddDays(-3), MortalityCount = 8, FeedConsumedKg = 1920, WaterConsumedLiters = 3850, EggsCollected = 0, DamagedEggs = 0, AverageWeightGrams = 1840, Notes = "Ventilation adjusted for nighttime humidity.", RecordedBy = farmManager.FullName },
                    new() { BatchId = batch1.BatchId, LogDate = DateTime.UtcNow.Date.AddDays(-2), MortalityCount = 5, FeedConsumedKg = 1980, WaterConsumedLiters = 3980, EggsCollected = 0, DamagedEggs = 0, AverageWeightGrams = 1920, Notes = "Vaccination booster administered via waterline.", RecordedBy = farmManager.FullName },
                    new() { BatchId = batch1.BatchId, LogDate = DateTime.UtcNow.Date.AddDays(-1), MortalityCount = 7, FeedConsumedKg = 2050, WaterConsumedLiters = 4100, EggsCollected = 0, DamagedEggs = 0, AverageWeightGrams = 2010, Notes = "Weight targets achieved ahead of schedule.", RecordedBy = farmManager.FullName },
                    new() { BatchId = batch1.BatchId, LogDate = DateTime.UtcNow.Date, MortalityCount = 4, FeedConsumedKg = 2100, WaterConsumedLiters = 4200, EggsCollected = 0, DamagedEggs = 0, AverageWeightGrams = 2100, Notes = "Daily biosecurity walkthrough complete.", RecordedBy = farmManager.FullName },

                    // Layer logs
                    new() { BatchId = batch2.BatchId, LogDate = DateTime.UtcNow.Date.AddDays(-2), MortalityCount = 2, FeedConsumedKg = 1680, WaterConsumedLiters = 3360, EggsCollected = 14020, DamagedEggs = 45, AverageWeightGrams = 1880, Notes = "Egg shell strength testing: 42 N average.", RecordedBy = farmManager.FullName },
                    new() { BatchId = batch2.BatchId, LogDate = DateTime.UtcNow.Date.AddDays(-1), MortalityCount = 3, FeedConsumedKg = 1700, WaterConsumedLiters = 3400, EggsCollected = 14110, DamagedEggs = 38, AverageWeightGrams = 1890, Notes = "Egg collection grading completed.", RecordedBy = farmManager.FullName },
                    new() { BatchId = batch2.BatchId, LogDate = DateTime.UtcNow.Date, MortalityCount = 1, FeedConsumedKg = 1710, WaterConsumedLiters = 3420, EggsCollected = 14190, DamagedEggs = 32, AverageWeightGrams = 1895, Notes = "Laying percentage 95.8%.", RecordedBy = farmManager.FullName }
                };
                await context.PoultryDailyLogs.AddRangeAsync(dailyLogs);

                // 8. Seed Visitors (Biosecurity Pillar A)
                var visitors = new List<Visitor>
                {
                    new() { FarmId = farm1.FarmId, VisitorName = "Jonathan Vance", ContactNumber = "+1 (515) 304-9912", Organization = "Midwest Feed Nutritionals Corp", PurposeOfVisit = "Bulk Feed Delivery & Silo Level Verification", EntryTime = DateTime.UtcNow.AddDays(-2).AddHours(9), ExitTime = DateTime.UtcNow.AddDays(-2).AddHours(11), VehicleNumber = "IA-TRK-9021", VisitedAnotherFarmRecently = false, DisinfectionConfirmed = true, ShowerTaken = true, ProtectiveClothingIssued = true, ApprovalStatus = ApprovalStatus.Approved, ApprovedBy = farmManager.FullName, Remarks = "Full truck undercarriage wash completed before entering perimeter buffer." },
                    new() { FarmId = farm1.FarmId, VisitorName = "Dr. Elena Rostova", ContactNumber = "+1 (515) 440-2211", Organization = "State Veterinary Inspection Service", PurposeOfVisit = "Quarterly Biosecurity Certification Audit", EntryTime = DateTime.UtcNow.AddDays(-5).AddHours(10), ExitTime = DateTime.UtcNow.AddDays(-5).AddHours(15), VehicleNumber = "GOV-VET-44", VisitedAnotherFarmRecently = true, LastFarmVisitedDetails = "County Research Facility (3 days prior, cleared)", DisinfectionConfirmed = true, ShowerTaken = true, ProtectiveClothingIssued = true, ApprovalStatus = ApprovalStatus.Approved, ApprovedBy = farmManager.FullName, Remarks = "All bio-containment gates and PPE rules satisfied." },
                    new() { FarmId = farm1.FarmId, VisitorName = "Robert Gallagher", ContactNumber = "+1 (515) 881-3344", Organization = "Gallagher Electrical Systems", PurposeOfVisit = "Emergency Backup Generator Service", EntryTime = DateTime.UtcNow.AddDays(-1).AddHours(14), ExitTime = DateTime.UtcNow.AddDays(-1).AddHours(17), VehicleNumber = "IA-SRV-5510", VisitedAnotherFarmRecently = false, DisinfectionConfirmed = true, ShowerTaken = false, ProtectiveClothingIssued = true, ApprovalStatus = ApprovalStatus.Approved, ApprovedBy = farmManager.FullName, Remarks = "Restricted to external power generation room; no animal contact." },
                    new() { FarmId = farm2.FarmId, VisitorName = "Claire Dunphy", ContactNumber = "+1 (770) 229-8811", Organization = "Georgia Department of Agriculture", PurposeOfVisit = "Avian Influenza Surveillance Sampling", EntryTime = DateTime.UtcNow.AddDays(-3).AddHours(8), ExitTime = DateTime.UtcNow.AddDays(-3).AddHours(13), VehicleNumber = "GA-AGR-102", VisitedAnotherFarmRecently = false, DisinfectionConfirmed = true, ShowerTaken = true, ProtectiveClothingIssued = true, ApprovalStatus = ApprovalStatus.Approved, ApprovedBy = "Farm Supervisor", Remarks = "Cloacal and tracheal swab collection for routine pathogen screening." }
                };
                await context.Visitors.AddRangeAsync(visitors);

                // 9. Seed Biosecurity Activities across Pillars
                var activities = new List<BiosecurityActivity>
                {
                    new() { FarmId = farm1.FarmId, Category = BiosecurityCategory.CleaningDisinfection, ActivityType = "Deep Barn Pressure Washing & Aerosol Virucide Fogging", ActivityDate = DateTime.UtcNow.AddDays(-1), ResponsiblePerson = farmManager.FullName, Description = "Terminal decontamination of Grower Pen 10 using glutaraldehyde compound after pig batch transfer.", ComplianceStatus = ComplianceStatus.Compliant, Remarks = "24-hour drying period enforced before restocking." },
                    new() { FarmId = farm1.FarmId, Category = BiosecurityCategory.VisitorManagement, ActivityType = "Vehicle Wash Bay Inspection & Chemical Renewal", ActivityDate = DateTime.UtcNow.AddDays(-2), ResponsiblePerson = farmManager.FullName, Description = "High-pressure nozzle cleaning and refill of quaternary ammonium concentrate at main perimeter gate.", ComplianceStatus = ComplianceStatus.Compliant, Remarks = "Spray pressure verified at 65 PSI." },
                    new() { FarmId = farm1.FarmId, Category = BiosecurityCategory.FeedWaterManagement, ActivityType = "Feed Silo Seal & Moisture Testing", ActivityDate = DateTime.UtcNow.AddDays(-3), ResponsiblePerson = "John Miller", Description = "Inspected Silos 1 through 4. Moisture levels measured below 13.5%; no caking or fungal growth detected.", ComplianceStatus = ComplianceStatus.Compliant, Remarks = "Passed all QA standards." },
                    new() { FarmId = farm1.FarmId, Category = BiosecurityCategory.PestRodentControl, ActivityType = "Perimeter Bait Station Inspection & Restocking", ActivityDate = DateTime.UtcNow.AddDays(-4), ResponsiblePerson = "Vector Control Team", Description = "All 28 exterior rodent bait stations inspected. 3 stations replenished with difenacoum wax blocks.", ComplianceStatus = ComplianceStatus.Compliant, Remarks = "Zero rodent activity observed inside animal barns." },
                    new() { FarmId = farm1.FarmId, Category = BiosecurityCategory.WasteManagement, ActivityType = "Slurry Bio-Digester Aeration & Perimeter Drainage Check", ActivityDate = DateTime.UtcNow.AddDays(-5), ResponsiblePerson = "Environmental Lead", Description = "Checked leachate barriers and lagoon containment depth; freeboard maintained at 1.4 meters.", ComplianceStatus = ComplianceStatus.Compliant, Remarks = "Zero runoff risk." },
                    new() { FarmId = farm2.FarmId, Category = BiosecurityCategory.AnimalHealthMonitoring, ActivityType = "Avian Flock Health & Respiratory Audit", ActivityDate = DateTime.UtcNow.AddDays(-1), ResponsiblePerson = vetOfficer.FullName, Description = "Conducted morning sound and behavior inspection across Sheds 1 to 4; zero respiratory rales detected.", ComplianceStatus = ComplianceStatus.Compliant, Remarks = "Flock health robust." }
                };
                await context.BiosecurityActivities.AddRangeAsync(activities);

                // 10. Seed Cleaning Records (Pillar B)
                var cleanings = new List<CleaningRecord>
                {
                    new() { FarmId = farm1.FarmId, FacilityArea = "Farrowing Barn A & Disinfection Airlock", EquipmentSanitized = "Crates, Heat lamps, Slats, Drinkers", VehicleDisinfectionStation = true, FootbathRefreshed = true, HandwashingSuppliesChecked = true, DisinfectantUsed = "Virkon S (Peroxygen compound)", DilutionRatio = "1:100", PerformedBy = farmManager.FullName, CleaningDate = DateTime.UtcNow.AddDays(-1), Status = "Verified", Notes = "Complete contact time 30 minutes achieved." },
                    new() { FarmId = farm1.FarmId, FacilityArea = "Perimeter Entry Gate & Shower Block", EquipmentSanitized = "Shower cubicles, Boot racks, Lockers", VehicleDisinfectionStation = true, FootbathRefreshed = true, HandwashingSuppliesChecked = true, DisinfectantUsed = "Glutaraldehyde & QAC blend", DilutionRatio = "1:200", PerformedBy = "Sanitation Crew", CleaningDate = DateTime.UtcNow.AddDays(-2), Status = "Verified", Notes = "Boot wash solution concentration tested at 1.8%." },
                    new() { FarmId = farm2.FarmId, FacilityArea = "Broiler Shed #1 Pre-Placement Sanitization", EquipmentSanitized = "Augers, Pan feeders, Nipple drinker lines", VehicleDisinfectionStation = true, FootbathRefreshed = true, HandwashingSuppliesChecked = true, DisinfectantUsed = "Formaldehyde-Free Virucide", DilutionRatio = "1:150", PerformedBy = "Poultry Hygiene Team", CleaningDate = DateTime.UtcNow.AddDays(-3), Status = "Verified", Notes = "Microbial swab tests came back negative." }
                };
                await context.CleaningRecords.AddRangeAsync(cleanings);

                // 11. Seed Feed & Water Quality Records (Pillar D)
                var feedRecords = new List<FeedRecord>
                {
                    new() { FarmId = farm1.FarmId, InspectionDate = DateTime.UtcNow.AddDays(-2), FeedType = "Swine Grower Pellets (16% Protein)", BatchLotNumber = "LT-2025-SW902", SupplierName = "Midwest Feed Nutritionals Corp", QuantityKg = 8500, QualityCheckPassed = true, MoistureLevelOk = true, NoMoldDetected = true, StorageSiloCondition = "Sealed, clean, temperature 18°C", InspectedBy = farmManager.FullName, Remarks = "Certificate of Analysis for mycotoxins verified negative." },
                    new() { FarmId = farm2.FarmId, InspectionDate = DateTime.UtcNow.AddDays(-4), FeedType = "Broiler Finisher Crumbles", BatchLotNumber = "LT-2025-BR441", SupplierName = "Purina Agri-Feed Solutions", QuantityKg = 14000, QualityCheckPassed = true, MoistureLevelOk = true, NoMoldDetected = true, StorageSiloCondition = "Ventilated steel silo, moisture 12.1%", InspectedBy = "Feed Supervisor", Remarks = "Particle size uniformity meets specification." }
                };
                await context.FeedRecords.AddRangeAsync(feedRecords);

                var waterRecords = new List<WaterQualityRecord>
                {
                    new() { FarmId = farm1.FarmId, TestingDate = DateTime.UtcNow.AddDays(-3), WaterSource = "Deep Artesian Well #2", PhLevel = 6.9, ChlorinePpm = 3.2, TdsLevel = 380, ColiformBacterialTestResult = "0 CFU / 100ml (Negative)", SanitizationMethod = "Automated Sodium Hypochlorite Dosing & 5-Micron Filtration", IsPotableSafe = true, TestedBy = farmManager.FullName, Remarks = "Optimal water quality for pig health." },
                    new() { FarmId = farm2.FarmId, TestingDate = DateTime.UtcNow.AddDays(-1), WaterSource = "Treated Municipal & Reserve Reservoir", PhLevel = 6.6, ChlorinePpm = 3.8, TdsLevel = 295, ColiformBacterialTestResult = "0 CFU / 100ml (Negative)", SanitizationMethod = "Chlorine Dioxide Injection & UV Reactor", IsPotableSafe = true, TestedBy = "Water Safety Officer", Remarks = "Flow rate consistent across all drinking lines." }
                };
                await context.WaterQualityRecords.AddRangeAsync(waterRecords);

                // 12. Seed Pest Control Records (Pillar E)
                var pestRecords = new List<PestControlRecord>
                {
                    new() { FarmId = farm1.FarmId, InspectionDate = DateTime.UtcNow.AddDays(-4), TargetPest = "Rats & House Mice (Rattus norvegicus)", BaitStationsChecked = 28, TrapsServiced = 14, InfestationLevel = "Low", ChemicalOrMethodUsed = "Brodifacoum weather-resistant tamper blocks & mechanical snap traps", CorrectiveActionTaken = "Sealed perimeter conduit pipe opening near barn 2", NextScheduledDate = DateTime.UtcNow.AddDays(10), InspectorName = "EcoPest Agricultural Services", Remarks = "No rodent sightings inside animal pens." },
                    new() { FarmId = farm2.FarmId, InspectionDate = DateTime.UtcNow.AddDays(-6), TargetPest = "Musca domestica (Houseflies) & Darkling Beetles", BaitStationsChecked = 35, TrapsServiced = 20, InfestationLevel = "None", ChemicalOrMethodUsed = "Neporex larvicide granules in manure pits & pyrethroid misting", CorrectiveActionTaken = "Routine preventive application", NextScheduledDate = DateTime.UtcNow.AddDays(8), InspectorName = "Apex Biopest Control Inc", Remarks = "Zero beetle larval emergence detected." }
                };
                await context.PestControlRecords.AddRangeAsync(pestRecords);

                // 13. Seed Waste Management Records (Pillar F)
                var wasteRecords = new List<WasteManagementRecord>
                {
                    new() { FarmId = farm1.FarmId, DisposalDate = DateTime.UtcNow.AddDays(-2), WasteType = "Manure & Slurry", DisposalMethod = "Anaerobic Digester & Sub-surface Soil Injection", QuantityKg = 12500, SanitationOfDisposalAreaChecked = true, DisposalDepthOrTemp = "30 cm subsurface injection depth", SupervisorName = farmManager.FullName, Remarks = "Complies with EPA agricultural nutrient management plan." },
                    new() { FarmId = farm1.FarmId, DisposalDate = DateTime.UtcNow.AddDays(-5), WasteType = "Mortality Carcass", DisposalMethod = "High-Temperature Bio-Incineration", QuantityKg = 45, SanitationOfDisposalAreaChecked = true, DisposalDepthOrTemp = "850°C primary combustion chamber", SupervisorName = farmManager.FullName, Remarks = "Complete ash reduction; bio-secure exhaust scrubbing." },
                    new() { FarmId = farm2.FarmId, DisposalDate = DateTime.UtcNow.AddDays(-1), WasteType = "Poultry Litter & Manure", DisposalMethod = "Covered High-Heat Aerated Composting", QuantityKg = 18000, SanitationOfDisposalAreaChecked = true, DisposalDepthOrTemp = "68°C Thermophilic stage (pathogen kill)", SupervisorName = "Waste Specialist", Remarks = "Temperature log recorded twice daily." }
                };
                await context.WasteManagementRecords.AddRangeAsync(wasteRecords);

                // 14. Seed Digital Biosecurity Inspections
                var allItems = await context.InspectionChecklistItems.OrderBy(i => i.SortOrder).ToListAsync();

                // Inspection 1: Farm 1 (Completed, High score)
                var inspection1 = new Inspection
                {
                    FarmId = farm1.FarmId,
                    InspectionCode = "INS-2025-PIG01",
                    InspectionDate = DateTime.UtcNow.AddDays(-5),
                    InspectorId = vetOfficer.Id,
                    InspectorName = vetOfficer.FullName,
                    InspectorDesignation = "Senior Veterinary Field Officer",
                    TotalChecklistItems = 20,
                    CompliantItems = 19,
                    NonCompliantItems = 1,
                    NotApplicableItems = 0,
                    CompliancePercentage = 95.0, // (19/20)*100
                    RiskLevel = RiskLevel.Low,
                    Status = InspectionStatus.Completed,
                    CertifiedBiosecure = true,
                    ViolationsIdentified = "Minor: North perimeter footbath concentration fell below 1.5% dilution due to rainwater dilution.",
                    Recommendations = "Install weather covers over outdoor footbath stations and increase chemical inspection frequency to twice daily.",
                    CorrectiveActionRequired = "Weather hoods mounted on footbaths 1 and 2 within 48 hours.",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                };
                await context.Inspections.AddAsync(inspection1);
                await context.SaveChangesAsync();

                // Responses for Inspection 1
                for (int i = 0; i < allItems.Count; i++)
                {
                    var item = allItems[i];
                    var isViolation = (i == 5); // Item 6 (Footbath)
                    var response = new InspectionResponse
                    {
                        InspectionId = inspection1.InspectionId,
                        ChecklistItemId = item.ItemId,
                        Status = isViolation ? ComplianceStatus.NonCompliant : ComplianceStatus.Compliant,
                        Remarks = isViolation ? "Solution diluted by overnight precipitation." : "Exceeds required biosecurity standard.",
                        CorrectiveAction = isViolation ? "Fresh virucide solution reconstituted and rain canopy scheduled for installation." : null
                    };
                    await context.InspectionResponses.AddAsync(response);
                }

                // Inspection 2: Farm 2 (Completed, 85%)
                var inspection2 = new Inspection
                {
                    FarmId = farm2.FarmId,
                    InspectionCode = "INS-2025-PLT02",
                    InspectionDate = DateTime.UtcNow.AddDays(-12),
                    InspectorId = vetOfficer.Id,
                    InspectorName = vetOfficer.FullName,
                    InspectorDesignation = "Senior Veterinary Field Officer",
                    TotalChecklistItems = 20,
                    CompliantItems = 17,
                    NonCompliantItems = 3,
                    NotApplicableItems = 0,
                    CompliancePercentage = 85.0,
                    RiskLevel = RiskLevel.Medium,
                    Status = InspectionStatus.Completed,
                    CertifiedBiosecure = true,
                    ViolationsIdentified = "1. Mesh screen tear on shed 2 east louver. 2. Visitor sign-out time missing on 2 logs. 3. Bait station 14 displaced.",
                    Recommendations = "Immediate repair of bird netting; automated visitor badge scanning; anchor all bait stations to concrete pads.",
                    CorrectiveActionRequired = "Mesh replacement completed by maintenance team within 24 hours.",
                    CreatedAt = DateTime.UtcNow.AddDays(-12)
                };
                await context.Inspections.AddAsync(inspection2);
                await context.SaveChangesAsync();

                // Responses for Inspection 2
                for (int i = 0; i < allItems.Count; i++)
                {
                    var item = allItems[i];
                    var isViolation = (i == 2 || i == 14 || i == 15);
                    var response = new InspectionResponse
                    {
                        InspectionId = inspection2.InspectionId,
                        ChecklistItemId = item.ItemId,
                        Status = isViolation ? ComplianceStatus.NonCompliant : ComplianceStatus.Compliant,
                        Remarks = isViolation ? "Identified deviation requiring corrective remediation." : "Meets state animal health standards.",
                        CorrectiveAction = isViolation ? "Action logged in corrective action tracker." : null
                    };
                    await context.InspectionResponses.AddAsync(response);
                }

                // 15. Seed Disease Reports
                var disease1 = new DiseaseReport
                {
                    FarmId = farm1.FarmId,
                    AnimalId = pigs[6].AnimalId, // PG-0207
                    DiseaseName = "Porcine Respiratory Disease Complex (PRDC)",
                    DiseaseCategory = "Bacterial / Viral Synergistic",
                    DetectionDate = DateTime.UtcNow.AddDays(-4),
                    Severity = DiseaseSeverity.Medium,
                    Symptoms = "Abdominal thumping, persistent dry cough, pyrexia (40.2°C), decreased feed intake.",
                    AffectedCount = 3,
                    MortalityCount = 0,
                    IsolationImplemented = true,
                    TreatmentGiven = "Tilmicosin injectable 10 mg/kg + oral electrolyte hydration in isolated quarantine pen.",
                    VerificationStatus = DiseaseVerificationStatus.UnderInvestigation,
                    VeterinaryNotes = "Lung auscultation reveals bilateral wheezing. Nasal swabs sent for PCR sequencing. Quarantine containment strictly enforced.",
                    VerifiedBy = vetOfficer.FullName,
                    VerificationDate = DateTime.UtcNow.AddDays(-3),
                    ReportedBy = farmManager.FullName,
                    ReportDate = DateTime.UtcNow.AddDays(-4)
                };

                var disease2 = new DiseaseReport
                {
                    FarmId = farm2.FarmId,
                    PoultryBatchId = batch1.BatchId,
                    DiseaseName = "Subclinical Coccidiosis (Eimeria acervulina)",
                    DiseaseCategory = "Parasitic",
                    DetectionDate = DateTime.UtcNow.AddDays(-15),
                    Severity = DiseaseSeverity.Low,
                    Symptoms = "Mucoid droppings, mild feather ruffling, 4% temporary drop in daily weight gain curve.",
                    AffectedCount = 85,
                    MortalityCount = 4,
                    IsolationImplemented = true,
                    TreatmentGiven = "Toltrazuril 7 mg/kg administered via drinking water lines for 48 hours.",
                    VerificationStatus = DiseaseVerificationStatus.Resolved,
                    VeterinaryNotes = "Post-treatment oocyst fecal floatation negative. Full flock recovered; FCR returned to target trajectory.",
                    VerifiedBy = vetOfficer.FullName,
                    VerificationDate = DateTime.UtcNow.AddDays(-10),
                    ReportedBy = "Poultry Health Officer",
                    ReportDate = DateTime.UtcNow.AddDays(-15)
                };

                await context.DiseaseReports.AddRangeAsync(disease1, disease2);
                await context.SaveChangesAsync();

                // 16. Seed Vaccinations
                var vaccinations = new List<Vaccination>
                {
                    new() { FarmId = farm1.FarmId, AnimalId = pigs[0].AnimalId, VaccineName = "Porcilis PRRS Modified Live", DiseaseTargeted = "Porcine Reproductive & Respiratory Syndrome", BatchLotNumber = "VAC-PRRS-2025-01", VaccinationDate = DateTime.UtcNow.AddDays(-30), NextDueDate = DateTime.UtcNow.AddDays(150), DoseAdministered = "2.0 ml", Route = "Intramuscular", AdministeredBy = vetOfficer.FullName, Status = VaccinationStatus.Completed, Remarks = "Booster vaccination prior to breeding cycle." },
                    new() { FarmId = farm1.FarmId, AnimalId = pigs[1].AnimalId, VaccineName = "Suvaxyn PCV2 + Mycoplasma", DiseaseTargeted = "Porcine Circovirus Type 2 & Enzootic Pneumonia", BatchLotNumber = "VAC-PCV2-9901", VaccinationDate = DateTime.UtcNow.AddDays(-25), NextDueDate = DateTime.UtcNow.AddDays(155), DoseAdministered = "2.0 ml", Route = "Intramuscular", AdministeredBy = vetOfficer.FullName, Status = VaccinationStatus.Completed, Remarks = "Full immunity profile verified." },
                    new() { FarmId = farm1.FarmId, AnimalId = pigs[4].AnimalId, VaccineName = "Eryseng Parvo Swine", DiseaseTargeted = "Swine Erysipelas & Parvovirus", BatchLotNumber = "VAC-ERY-552", VaccinationDate = DateTime.UtcNow.AddDays(3), NextDueDate = DateTime.UtcNow.AddDays(180), DoseAdministered = "2.0 ml", Route = "Intramuscular", AdministeredBy = farmManager.FullName, Status = VaccinationStatus.Scheduled, Remarks = "Pre-scheduled grower immunization." },

                    // Poultry Vaccinations
                    new() { FarmId = farm2.FarmId, PoultryBatchId = batch1.BatchId, VaccineName = "Nobilis ND Clone 30", DiseaseTargeted = "Newcastle Disease", BatchLotNumber = "VAC-ND-7821", VaccinationDate = DateTime.UtcNow.AddDays(-20), NextDueDate = DateTime.UtcNow.AddDays(25), DoseAdministered = "1 dose per bird", Route = "Coarse Aerosol Spray", AdministeredBy = "Avian Tech Team", Status = VaccinationStatus.Completed, Remarks = "Batch-wide spray cabinet application at day 14." },
                    new() { FarmId = farm2.FarmId, PoultryBatchId = batch1.BatchId, VaccineName = "Cevac Transmune IBD", DiseaseTargeted = "Infectious Bursal Disease (Gumboro)", BatchLotNumber = "VAC-IBD-109", VaccinationDate = DateTime.UtcNow.AddDays(-28), NextDueDate = null, DoseAdministered = "1 dose", Route = "In Ovo / Subcutaneous", AdministeredBy = "Hatchery Vet Staff", Status = VaccinationStatus.Completed, Remarks = "Administered at hatchery on day of hatch." },
                    new() { FarmId = farm2.FarmId, PoultryBatchId = batch2.BatchId, VaccineName = "Infectious Bronchitis H120", DiseaseTargeted = "Avian Infectious Bronchitis", BatchLotNumber = "VAC-IB-902", VaccinationDate = DateTime.UtcNow.AddDays(-5), NextDueDate = DateTime.UtcNow.AddDays(55), DoseAdministered = "1 dose", Route = "Drinking Water", AdministeredBy = "Layer Tech Team", Status = VaccinationStatus.Completed, Remarks = "Stabilized with skim milk powder buffer." }
                };
                await context.Vaccinations.AddRangeAsync(vaccinations);

                // 17. Seed Corrective Actions
                var correctiveActions = new List<CorrectiveAction>
                {
                    new() { FarmId = farm1.FarmId, InspectionId = inspection1.InspectionId, Title = "Install Heavy-Duty Rain Shelters Over Outdoor Footbaths", Description = "Fabricate UV-resistant stainless steel protective canopies over North and East vehicle entrance footbaths to prevent rainwater chemical dilution.", AssignedTo = farmManager.FullName, DueDate = DateTime.UtcNow.AddDays(2), Status = "InProgress", VerificationNotes = "Materials ordered; installation scheduled for tomorrow morning." },
                    new() { FarmId = farm1.FarmId, DiseaseReportId = disease1.ReportId, Title = "Strict Bio-Quarantine Isolation for Barn B Affected Stock", Description = "Implement double barrier isolation, dedicated disposable overalls, and footbath for Isolation Pen Iso-1 until viral PCR results return.", AssignedTo = farmManager.FullName, DueDate = DateTime.UtcNow.AddDays(1), Status = "Completed", CompletedDate = DateTime.UtcNow.AddDays(-1), VerificationNotes = "Isolation confirmed and supervised by Dr. Rostova." }
                };
                await context.CorrectiveActions.AddRangeAsync(correctiveActions);

                // 18. Seed Notifications
                var notifications = new List<Notification>
                {
                    new() { FarmId = farm1.FarmId, Title = "Critical Health Watch: PRDC Suspected in Pen Iso-1", Message = "Pig Tag PG-0207 has been placed in strict isolation under Tilmicosin therapy. Nasal PCR tests pending.", Category = NotificationCategory.DiseaseAlert, Severity = NotificationSeverity.Warning, TargetUrl = "/Diseases", CreatedAt = DateTime.UtcNow.AddHours(-18), IsRead = false },
                    new() { FarmId = farm1.FarmId, Title = "Quarterly Biosecurity Audit Completed (Score: 95%)", Message = "Farm inspection INS-2025-PIG01 completed by Dr. Rostova. Rated Low Risk (Certified Biosecure). 1 minor corrective action logged.", Category = NotificationCategory.InspectionDue, Severity = NotificationSeverity.Success, TargetUrl = "/Inspections", CreatedAt = DateTime.UtcNow.AddDays(-2), IsRead = false },
                    new() { FarmId = farm1.FarmId, Title = "Upcoming Vaccination: Eryseng Parvo Scheduled", Message = "Scheduled vaccination for grower pig PG-0205 due in 3 days. Ensure vaccine cold-chain integrity.", Category = NotificationCategory.OverdueVaccination, Severity = NotificationSeverity.Info, TargetUrl = "/Vaccinations", CreatedAt = DateTime.UtcNow.AddHours(-5), IsRead = false },
                    new() { FarmId = farm2.FarmId, Title = "Feed Delivery Gate Clearance Logged", Message = "Purina Agri-Feed bulk tanker cleared through vehicle disinfection arch. Chassis wash verified.", Category = NotificationCategory.BiosecurityViolation, Severity = NotificationSeverity.Info, TargetUrl = "/Biosecurity/Visitors", CreatedAt = DateTime.UtcNow.AddDays(-1), IsRead = true }
                };
                await context.Notifications.AddRangeAsync(notifications);

                // 19. Seed Audit Logs
                var auditLogs = new List<AuditLog>
                {
                    new() { UserId = superAdmin.Id, UserName = superAdmin.FullName, Action = "System Initialization", EntityName = "System", EntityId = "ROOT", Details = "FarmShield Digital Biosecurity Management System initialized with WOAH/USDA biosecurity standard matrices.", Timestamp = DateTime.UtcNow.AddMonths(-12), IpAddress = "127.0.0.1" },
                    new() { UserId = vetOfficer.Id, UserName = vetOfficer.FullName, Action = "Audit Completed", EntityName = "Inspection", EntityId = "INS-2025-PIG01", Details = "Completed full 20-point digital biosecurity audit on Green Valley Biosecure Pig Facility. Compliance calculated at 95.0%.", Timestamp = DateTime.UtcNow.AddDays(-5), IpAddress = "192.168.1.45" },
                    new() { UserId = farmManager.Id, UserName = farmManager.FullName, Action = "Disease Case Logged", EntityName = "DiseaseReport", EntityId = "PG-0207", Details = "Logged respiratory symptom case for grower pig PG-0207 and triggered isolation quarantine protocol.", Timestamp = DateTime.UtcNow.AddDays(-4), IpAddress = "192.168.1.102" }
                };
                await context.AuditLogs.AddRangeAsync(auditLogs);

                await context.SaveChangesAsync();
            }
        }
    }
}
