using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class VerificationCodeTest
{
    private Mock<IDateTimeProvider> _mockDateTimeProvider;

    public VerificationCodeTest()
    {
        _mockDateTimeProvider = new Mock<IDateTimeProvider>();
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(new DateTime(2024, 10, 29));
    }

    [Fact]
    public void ShouldGenerateVerificationCode()
    {
        // Arrange
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.True(verificationCode.ExpiresAtUtc.HasValue);
    }

    [Fact]
    public void ShouldGenerateExpiresAtInFuture()
    {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(new DateTime(2070, 10, 29));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.True(verificationCode.ExpiresAtUtc > DateTime.Now);
    }

    [Fact]
    public void ShouldGenerateVerifiedAtAsNull()
    {
        // Arrange
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.True(verificationCode.VerifiedAtUtc is null);
    }

    [Fact]
    public void ShouldBeInactiveWhenCreated()
    {
        // Arrange
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.True(verificationCode.IsActive == false);
    }

    [Fact]
    public void ShouldFailIfExpired()
    {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(new DateTime(2024, 9, 29));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.True(verificationCode.ExpiresAtUtc < DateTime.Now);
    }

    [Fact]
    public void ShouldFailIfCodeIsInvalid()
    {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(new DateTime(2024, 9, 29));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.Throws<InvalidVerificationCodeException>(() => verificationCode.ShouldVerify(code));
    }
    [Fact]
    public void ShouldFailIfCodeIsLessThanSixChars() {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(DateTime.Now.AddDays(1));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.True(code.Length < 6);
    }

    [Fact]
    public void ShouldFailIfCodeIsGreaterThanSixChars()
    {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(DateTime.Now.AddDays(1));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.False(code.Length > 6);
    }

    [Fact]
    public void ShouldFailIfIsNotActive()
    {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(new DateTime(2024, 9, 29));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;

        // Assert
        Assert.False(verificationCode.IsActive);
    }

    [Fact]
    public void ShouldFailIfIsAlreadyVerified()
    {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(new DateTime(2024, 9, 29));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;
        verificationCode.ShouldVerify(code);

        // Assert
        Assert.False(verificationCode.VerifiedAtUtc is not null);
    }

    [Fact]
    public void ShouldFailIfIsVerificationCodeDoesNotMatch()
    {

    }

    [Fact]
    public void ShouldVerify()
    {
        // Arrange
        _mockDateTimeProvider.Setup(m => m.UtcNow).Returns(new DateTime(2024, 9, 29));
        var verificationCode = VerificationCode.ShouldCreate(_mockDateTimeProvider.Object);

        // Act
        var code = verificationCode.Code;
        verificationCode.ShouldVerify(code);

        // Assert
        Assert.True(verificationCode.VerifiedAtUtc > DateTime.Now);
    }
}