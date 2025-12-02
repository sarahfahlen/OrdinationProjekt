namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class DagligSkævTest
{
    private DataService service;
    private Laegemiddel lm = null!;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
        lm = service.GetLaegemidler().First();
    }
    
    [TestMethod]
    public void Antalover0()
    {
        Assert.IsTrue(service.GetDagligSkæve().Count() > 0);
    }

    [TestMethod]
    public void Antalunder0()
    {
        var skæv = service.GetDagligSkæve().First();

        try
        {
            skæv.opretDosis(DateTime.Now, -1);
            Assert.Fail("Forventede at opretDosis kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Antal må ikke være negativt.", ex.Message);
        }
    }

    [TestMethod]
    public void OpretDosis_NegativAntal_KasterException()
    {
        var skæv = service.GetDagligSkæve().First();

        try
        {
            skæv.opretDosis(DateTime.Now, -5);
            Assert.Fail("Forventede at opretDosis kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Antal må ikke være negativt.", ex.Message);
        }
    }
    
    [TestMethod]
    public void DagligSkaev_StartEfterSlut()
    {
        var laegemiddel = service.GetLaegemidler().First();

        try
        {
            var skæv = new DagligSkæv(
                new DateTime(2025, 2, 10), // start
                new DateTime(2025, 2, 1),  // slut
                laegemiddel
            );
            Assert.Fail("Forventede at konstruktøren kastede en exception");
        }
        catch (ArgumentException ex)
        {
            Assert.AreEqual("Startdato må ikke være efter slutdato.", ex.Message);
        }
    }


    [TestMethod]
    public void Skæv_SamletDosis_Korrekt()
    {
        var skæv = new DagligSkæv(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 3),
            lm
        );

        skæv.doser.Clear();

        skæv.opretDosis(DateTime.Now, 2);
        skæv.opretDosis(DateTime.Now, 3);
        
        double resultat = skæv.samletDosis();
        
        Assert.AreEqual(15, resultat);
    }

    [TestMethod]
    public void DoegnDosis_UdenDoser_Giver0()
    {
        var skæv = new DagligSkæv(DateTime.Now, DateTime.Now, lm);

        skæv.doser.Clear(); // sikrer ingen doser
        
        double resultat = skæv.doegnDosis();
        
        Assert.AreEqual(0, resultat);
    }

    [TestMethod]
    public void DoegnDosis_MedEnDosis_Korrekt()
    {
        var skæv = new DagligSkæv(DateTime.Now, DateTime.Now, lm);

        skæv.doser.Clear();
        skæv.opretDosis(DateTime.Now, 4.5);
        
        double resultat = skæv.doegnDosis();
        
        Assert.AreEqual(4.5, resultat);
    }
    
    [TestMethod]
    public void DoegnDosis_MedFlereDoser_Korrekt()
    {
        var skæv = new DagligSkæv(DateTime.Now, DateTime.Now, lm);

        skæv.doser.Clear();
        skæv.opretDosis(DateTime.Now, 1);
        skæv.opretDosis(DateTime.Now, 2.5);
        skæv.opretDosis(DateTime.Now, 3);
        // doegnDosis = 1 + 2.5 + 3 = 6.5
        
        double resultat = skæv.doegnDosis();
        
        Assert.AreEqual(6.5, resultat);
    }

    [TestMethod]
    public void GetType_Skæv()
    {
        var skæv = new DagligSkæv(
            DateTime.Now,
            DateTime.Now,
            lm
        );
        
        string type = skæv.getType();
        
        Assert.AreEqual("DagligSkæv", type);
    }

}