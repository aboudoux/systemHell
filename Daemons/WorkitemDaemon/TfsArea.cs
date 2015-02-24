using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace WorkitemDaemon
{
    public class TfsArea
    {
        public string AreaName { get; private set; }
        public string AreaPath { get; private set; }
        public int Id { get; private set; }
        public int ParentId { get; private set; }

        public TfsArea(Node areaNode)
        {
            if (!areaNode.IsAreaNode)
                throw new Exception("Le noeud passé n'est pas une zone TFS");
            Id = areaNode.Id;
            AreaName = areaNode.Name;
            AreaPath = areaNode.Path;
            ParentId = (areaNode.ParentNode != null) ? areaNode.ParentNode.Id : 0;
        }
    }
}