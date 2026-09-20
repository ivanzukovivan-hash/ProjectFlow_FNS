using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectFlow_FNS.Helpers;

namespace ProjectFlow_FNS.Tests.Helpers
{
    [TestClass]
    public class PasswordHasherTests
    {
        [TestMethod]
        public void Hash_SameInput_ReturnsSameHash()
        {
            string hash1 = PasswordHasher.Hash("1234");
            string hash2 = PasswordHasher.Hash("1234");
            Assert.AreEqual(hash1, hash2);
        }

        [TestMethod]
        public void Hash_DifferentInput_ReturnsDifferentHash()
        {
            string hash1 = PasswordHasher.Hash("1234");
            string hash2 = PasswordHasher.Hash("5678");
            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void Hash_ReturnsNonEmptyString()
        {
            string hash = PasswordHasher.Hash("test");
            Assert.IsFalse(string.IsNullOrEmpty(hash));
        }

        [TestMethod]
        public void Verify_CorrectPassword_ReturnsTrue()
        {
            string hash = PasswordHasher.Hash("1234");
            Assert.IsTrue(PasswordHasher.Verify("1234", hash));
        }

        [TestMethod]
        public void Verify_WrongPassword_ReturnsFalse()
        {
            string hash = PasswordHasher.Hash("1234");
            Assert.IsFalse(PasswordHasher.Verify("wrong", hash));
        }

        [TestMethod]
        public void Hash_IsCaseSensitive()
        {
            string hash1 = PasswordHasher.Hash("Password");
            string hash2 = PasswordHasher.Hash("password");
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}