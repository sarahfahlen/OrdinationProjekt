namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class DagligFastTest
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
    public void Doegndosis_Returnkorrektsum()
    {
        //arrange
        var laeg = new Laegemiddel("Test", 1, 1, 1, "ml");
        var df = new DagligFast(
            DateTime.Today,
            DateTime.Today.AddDays(1),
            laeg,
            morgenAntal: 2,
            middagAntal: 1,
            aftenAntal: 3,
            natAntal: 0);

        //act
        double result = df.doegnDosis();

        //assert
        Assert.AreEqual(6, result);
    }

    [TestMethod]
    public void Samletdosis_returnerkorrektValue()
    {
        // Arrange
        var laeg = new Laegemiddel("TestLaeg", 1, 1, 1, "ml");

        //  3 days
        var df = new DagligFast(
            new DateTime(2025, 1, 1),
            new DateTime(2025, 1, 3),
            laeg,
            morgenAntal: 2,
            middagAntal: 1,
            aftenAntal: 1,
            natAntal: 0); // doegn = 4

        // Act
        double result = df.samletDosis();

        // Assert
        Assert.AreEqual(12, result); // 4 * 3 = 12
    }
    [TestMethod]
    public void GetDoser() 
    {
        //Arrange
        var laeg = new Laegemiddel("TestMed", 1, 1, 1, "styk");
        var df = new DagligFast(
            DateTime.Today,
            DateTime.Today.AddDays(1),
            laeg,
            morgenAntal: 2,
            middagAntal: 1,
            aftenAntal: 3,
            natAntal: 4);

        //Act
        var doser = df.getDoser();
        
        //Assert
        Assert.AreEqual(df.MorgenDosis, doser[0], "MorgenDosis skulle være først");
        Assert.AreEqual(df.MiddagDosis, doser[1], "MiddagDosis skulle være anden");
        Assert.AreEqual(df.AftenDosis, doser[2], "AftenDosis skulle være tredje");
        Assert.AreEqual(df.NatDosis, doser[3], "NatDosis skulle være fjerde");
    }

    [TestMethod]
    public void GetType_Dagligfast()
    {
        //Arrange
        var laegemiddel = service.GetLaegemidler().First();
        var dagligfast = new DagligFast(DateTime.Now, DateTime.Now,laegemiddel, morgenAntal: 1,middagAntal: 1, aftenAntal: 1, natAntal: 1);
        
        //act
        string type = dagligfast.getType();  

        //arrange
        Assert.AreEqual("DagligFast", type);
    }

}