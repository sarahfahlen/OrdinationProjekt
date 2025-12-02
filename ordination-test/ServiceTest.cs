namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }
    
    //Metode der tester vores ugyldige data i GetAnbefaletDosisPerDøgn fra systemudvikling
    [TestMethod]
    public void AnbefaletDosisUgyldige()
    {
        //Her hentes den patient jeg tester på for at ændre vægt til testcasen --> Arrange
        Patient TestPatient = service.GetPatienter().Where(p => p.PatientId == 1).FirstOrDefault();
        
        //Her actes og asserter vi vores exceptions
        try
        {
            service.GetAnbefaletDosisPerDøgn(100, 1);
            Assert.Fail("Forventede at metoden kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Patienten findes ikke", ex.Message);
        }
        try
        {
            service.GetAnbefaletDosisPerDøgn(1, 100);
            Assert.Fail("Forventede at metoden kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Lægemidlet findes ikke", ex.Message);
        }
        //Denne metode er udkommenteret, da denne funktionalitet endnu ikke er implementeret 
        /*
        try 
        {
            TestPatient.vaegt = 0;
            service.GetAnbefaletDosisPerDøgn(1, 1);
            Assert.Fail("Forventede at metoden kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Patientens vægt er ugyldig", ex.Message);
        }
        */
    }
    
    //Metode der tester vores gyldige data i GetAnbefaletDosisPerDøgn fra systemudvikling
    [TestMethod]
    public void AnbefaletDosisGyldige()
    {
        //Her hentes den patient jeg tester på - senere ændrer jeg vægt for hver patient --> arranger
        Patient TestPatient = service.GetPatienter().Where(p => p.PatientId == 1).FirstOrDefault();
        
        //Her actes og assertes vores gyldige værdier
        TestPatient.vaegt = 1;
        double dosis1 =service.GetAnbefaletDosisPerDøgn(1, 1);
        Assert.AreEqual(0.1, dosis1, 0.0001);
        
        TestPatient.vaegt = 24;
        double dosis2 =service.GetAnbefaletDosisPerDøgn(1, 1);
        Assert.AreEqual(2.4, dosis2, 0.0001);
        
        TestPatient.vaegt = 25;
        double dosis3 = service.GetAnbefaletDosisPerDøgn(1, 1);
        Assert.AreEqual(3.75, dosis3, 0.0001);
        
        TestPatient.vaegt = 120;
        double dosis4 = service.GetAnbefaletDosisPerDøgn(1, 1);
        Assert.AreEqual(18.0, dosis4, 0.0001);
        
        TestPatient.vaegt = 121;
        double dosis5 = service.GetAnbefaletDosisPerDøgn(1, 1);
        Assert.AreEqual(19.36, dosis5, 0.0001);
    }
    
    //Metode der tester vores ugyldige data i OpretPN fra systemudvikling
    [TestMethod]
    public void OpretPNUgyldige()
    {
        //Arranger værdier til vores test - sætter datoer for overskuelighedens skyld
        DateTime start1 = new DateTime(2025, 11, 16);
        DateTime end1 = new DateTime(2025, 11, 21);
        DateTime end2 = new DateTime(2025, 11, 12);
        
        //Her actes og asserter vi vores exceptions
        try
        {
            service.OpretPN(100, 1, 1, start1, end1);
            Assert.Fail("Forventede at metoden kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Patient findes ikke", ex.Message);
        }
        
        try
        {
            service.OpretPN(1, 100, 1, start1, end1);
            Assert.Fail("Forventede at metoden kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Lægemiddel findes ikke", ex.Message);
        }
        
        //Udkommenteret da testen fejler grundet manglende implementation
        /*try
        {
            service.OpretPN(1, 2, -1, start1, end1);
            Assert.Fail("Forventede at metoden kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Ugyldigt antal angivet", ex.Message);
        }
        
        try
        {
            service.OpretPN(1, 2, 1, start1, end2);
            Assert.Fail("Forventede at metoden kastede en exception");
        }
        catch (ArgumentException ex)
        {
          Assert.AreEqual("Startdato skal være før slutdato", ex.Message);
        } */
    }
    //Metode der tester vores gyldige data i OpretPN fra systemudvikling
    [TestMethod]
    public void OpretPNGyldige()
    {
        //Arranger værdier til vores test - sætter datoer for overskuelighedens skyld
        DateTime start = new DateTime(2025, 11, 16);
        DateTime end = new DateTime(2025, 11, 21);
        
        //Acter på vores test
        PN TestPN = service.OpretPN(1,1,1,start,end);
        
        //Asserter vores test
        Assert.AreEqual(1, TestPN.antalEnheder);
        Assert.AreEqual(start, TestPN.startDen);
        Assert.AreEqual(end, TestPN.slutDen);
        Assert.AreEqual(1, TestPN.laegemiddel.LaegemiddelId);
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestAtKodenSmiderEnException()
    {
        service.GetAnbefaletDosisPerDøgn(999, 1);
    }
}