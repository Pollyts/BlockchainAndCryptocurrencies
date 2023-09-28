using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    static void Main()
    {
        var blockChain = new BlockChain();

        //starting a blockchain with the first block
        var currentHash = blockChain.AddNewBlock("HelloWorld", "");

        var answer = "0";
        while (answer != "8")
        {
            Console.WriteLine("1 –– Add a new block\n2 –– Show the last block\n3 –– Show the whole blockchain\n4 –– Verify the blockchain\n5 –– Verify and print the blockchain\n6 –– Change data of particular block\n7 –– Show number of blocks\n8 –– End the script \n");
            answer = Console.ReadLine();
            switch (answer)
            {
                case "1":
                    Console.WriteLine("Write your data");
                    var data = Console.ReadLine();
                    currentHash = blockChain.AddNewBlock(data, currentHash);
                    break;
                case "2":
                    blockChain.ShowBlock(currentHash);
                    break;
                case "3":
                    blockChain.ShowBlockChain();
                    break;
                case "4":
                    blockChain.Verify(currentHash);
                    break;
                case "5":
                    blockChain.VerifyAndShow();
                    break;
                case "6":
                    Console.WriteLine("Write the hash");
                    var hash = Console.ReadLine();
                    Console.WriteLine("Write your data");
                    var newData = Console.ReadLine();
                    blockChain.ChangeBlockData(hash, newData);
                    break;
                case "7":
                    blockChain.GetBlocksCount();
                    break;
                case "8":
                    break;
                default:
                    answer = "9";
                    break;
            }
        }
    }
}

public class BlockChain
{
    public static string GetHash(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        char[] charArray = text.ToCharArray();

        for (int i = 0; i < charArray.Length; i++)
        {
            if (char.IsLetter(charArray[i]))
            {
                char baseChar = char.IsUpper(charArray[i]) ? 'A' : 'a';
                charArray[i] = (char)(((charArray[i] - baseChar + 30) % 26) + baseChar);
            }
        }

        return new string(charArray);
    }
    public BlockChain()
    {
        Blocks = new List<Block>();
    }
    public List<Block> Blocks { get; set; }

    /// <summary>
    /// Adds a new block to the BlockChain
    /// </summary>
    /// <param name="data"></param>
    /// <param name="previousHash">if I understood right, in real world we are searching the previous block using previousHash, 
    /// but in that example I don`t need it because I am using List and the previous block will be always the last</param>
    /// <returns></returns>
    public string AddNewBlock(object data, string previousHash)
    {
        var hash = GetHash(data.ToString());
        try
        {

            Blocks.Add(new Block()
            {
                Data = data.ToString(),
                Timestamp = DateTime.Now,
                Hash = hash,
                Prev_Block_Hash = previousHash
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:" + ex.ToString());
        }

        Console.WriteLine("Block was added:");
        return hash;
    }

    public void ShowBlock(string previousHash)
    {
        var block = Blocks.Where(b => b.Hash == previousHash).FirstOrDefault();
        if (block != null)
        {
            Console.WriteLine(string.Format("Block with data {0} and hash {1} was created at {2}", block.Data, block.Hash, block.Timestamp.ToString()));
        }
        else
        {
            Console.WriteLine("Block hasn`t found");
        }
    }

    public void ShowBlockChain()
    {
        foreach (var block in Blocks)
        {
            ShowBlock(block.Hash);
        }
    }

    public bool Verify(string previousHash)
    {
        var block = Blocks.Where(b => b.Hash == previousHash).FirstOrDefault();
        var hash = GetHash(block.Data.ToString());

        if (previousHash == hash)
        {
            Console.WriteLine("Verified");
            return true;
        }
        else
        {
            Console.WriteLine("Unverified");
            return false;
        }
    }

    public void VerifyAndShow()
    {
        foreach (var block in Blocks)
        {
            if (Verify(block.Hash))
            {
                ShowBlock(block.Hash);
            }
        }
    }

    public void ChangeBlockData(string blockHash, object newData)
    {
        var block = Blocks.Where(b => b.Hash == blockHash).FirstOrDefault();

        if (block != null)
        {
            block.Data = newData.ToString();
            block.Hash = GetHash(block.Data.ToString());
        }
        else
        {
            Console.WriteLine("Block not found");
        }
    }

    public void GetBlocksCount()
    {
        Console.WriteLine(Blocks.Count.ToString());
    }
}

public class Block //Can`t it be a transaction?
{
    public string Hash { get; set; } //Unique hash of the block
    public string Data { get; set; } //Data of the transaction
    public DateTime Timestamp { get; set; }//Time of the transaction

    public string Prev_Block_Hash { get; set; } //Hash of the previous block

}
