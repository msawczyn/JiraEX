﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraRESTClient.Model
{
    public class BoardProject
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }

    }
}
