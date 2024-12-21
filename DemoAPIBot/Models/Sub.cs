using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Transactions;
using DemoAPIBot.Data;
using DemoAPIBot.Models;

namespace DemoAPIBot.Models
{
    [Index(nameof(Id))]
    public class Sub
    {
        public Sub()
        {
        }

        public Sub(int id)
        {
            Id = id;
        }
        public int Id { get; set; } //id del subscriber, va bene che sia incrementale e lo gestisca l'ef
        public List<SubMachine> subMachines { get; set; }

    }
}
