using Moq;
using Services.Services;
using Domain.IRepository;
using DTO.IDTO;
using Domain.Entities;
using DTO.DTO;
using ExternalServices.IServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TestApplication;

[TestClass]
public class BankServiceTests
{
    private Mock<IEsettService>? _mockEsettService;
    private Mock<IRepositoryBank>? _mockRepositoryBank;
    private Mock<ILogger<BankService>>? _mockLogger;

    private BankService? _bankService;

    [TestInitialize]
    public void Setup()
    {
        _mockEsettService = new Mock<IEsettService>();
        _mockRepositoryBank = new Mock<IRepositoryBank>();
        _mockLogger = new Mock<ILogger<BankService>>(); 
        _bankService = new BankService(_mockEsettService.Object, _mockRepositoryBank.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetBanksAsync_ReturnsBanksFromExternalApi()
    {
        // Arrange
        var bankDtos = new List<IBankDTO>
        {
            new BankDTO { Name = "Bank1", Bic = "BIC1", Country = "Country1" },
            new BankDTO { Name = "Bank2", Bic = "BIC2", Country = "Country2" }
        };
        _mockEsettService!.Setup(service => service.GetBanksAsync()).ReturnsAsync(bankDtos);

        // Act
        var result = await _bankService!.GetBanksAsync();

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("Bank1", result.First().Name);
    }

    [TestMethod]
    public async Task AddBankListAsync_AddsBanksToRepository()
    {
        // Arrange
        var bankDtos = new List<BankDTO>
    {
        new BankDTO { Name = "Bank1", Bic = "BIC1", Country = "US" },
        new BankDTO { Name = "Bank2", Bic = "BIC2", Country = "FR" }
    };

        // Act
        await _bankService!.AddBankListAsync(bankDtos);

        // Assert
        _mockRepositoryBank!.Verify(repo => repo.AddBankListAsync(It.IsAny<IEnumerable<Bank>>()), Times.Once);
    }



    [TestMethod]
    public async Task GetBanksFromDatabaseAsync_ReturnsBanksFromDatabase()
    {
        // Arrange
        var banks = new List<Bank>
        {
            new Bank { Id = 1, Name = "Bank1", Bic = "BIC1", Country = "Country1" },
            new Bank { Id = 2, Name = "Bank2", Bic = "BIC2", Country = "Country2" }
        };
        _mockRepositoryBank!.Setup(repo => repo.GetAllBanksFromDataBaseAsync()).ReturnsAsync(banks);

        // Act
        var result = await _bankService!.GetBanksFromDatabaseAsync();

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("Bank1", result.First().Name);
    }

    [TestMethod]
    public async Task GetBankById_ReturnsBank_WhenBankExists()
    {
        // Arrange
        var bank = new Bank { Id = 1, Name = "Bank1", Bic = "BIC1", Country = "Country1" };
        _mockRepositoryBank!.Setup(repo => repo.GetBankById(1)).ReturnsAsync(bank);

        // Act
        var result = await _bankService!.GetBankById(1);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Bank1", result.Name);
    }

    [TestMethod]
    public async Task GetBankById_ReturnsNull_WhenBankDoesNotExist()
    {
        // Arrange
        Bank? bankNull = null;
        _mockRepositoryBank!.Setup(repo => repo.GetBankById(1)).ReturnsAsync(bankNull);

        // Act
        var result = await _bankService!.GetBankById(1);

        // Assert
        Assert.IsNull(result);
    }




}
