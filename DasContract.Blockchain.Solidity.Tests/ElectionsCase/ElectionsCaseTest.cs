﻿using DasContract.Blockchain.Solidity.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DasContract.Blockchain.Solidity.Tests.ElectionsCase
{
    public class ElectionsCaseTest
    {
        [Fact]
        public void TestConversion()
        {
            var contract = ElectionContractFactory.CreateContract();

            var contractConverter = new ContractConverter(contract);
            contractConverter.ConvertContract();
            var solidityCode = contractConverter.GetSolidityCode();
            Console.WriteLine(solidityCode);

            System.IO.File.WriteAllText(@"C:\Users\Johny\Desktop\code.txt", solidityCode);

        }
    }
}
