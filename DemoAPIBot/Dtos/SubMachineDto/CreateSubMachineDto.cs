﻿using DemoAPIBot.Models;

namespace DemoAPIBot.Dtos.SubMachineDto
{
    public class CreateSubMachineDto
    {
        public int SubId { get; set; }
        public string MacchinaId { get; set; }
        public int levelPriority { get; set; }
        public string serviceDispatcher { get; set; }
    }
}