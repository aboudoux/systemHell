using System;
using System.Collections.Generic;
using System.IO;
using WorkitemDaemon.Helpers;

namespace WorkitemDaemon
{
    public enum EnumWorkitemType
    {
        Task,
        Bug
    }

    [Serializable]
    public class InternalWorkItem
    {
        public InternalWorkItem()
        {
            Attachements = new List<InternalAttachement>();
        }
        public InternalWorkItem(EnumWorkitemType Type, string Titre, string Description, string Rank = "", List<string> Attachement = null)
            : this()
        {
            this.Type = Type;
            this.Titre = Titre;
            this.Description = Description;
            this.Rank = Rank;
            if (Attachement != null)
            {
                foreach (var attachement in Attachement)
                {
                    AddAttachement(attachement);
                }
            }
        }
        public EnumWorkitemType Type { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public string Rank { get; set; }
        public List<InternalAttachement> Attachements { get; set; }
        public string Erreur { get; set; }

        public void AddAttachement(string filePath)
        {
            if (File.Exists(filePath))
                Attachements.Add(new InternalAttachement(filePath));
        }

        public IEnumerable<string> GetAttachementFiles()
        {
            foreach (var attachement in Attachements)
            {
                yield return attachement.GetFile();
            }
        }
    }

    [Serializable]
    public class InternalAttachement
    {
        public InternalAttachement() { }
        public InternalAttachement(string filePath)
        {
            Name = Path.GetFileName(filePath);
            Base64Data = Base64Helper.EncodeFile(filePath);
        }
        public string Name { get; set; }
        public string Base64Data { get; set; }

        public string GetFile()
        {
            if (string.IsNullOrEmpty(Name))
                throw new Exception("Le nom de la pièce jointe n'est pas attaché");
            string output = Path.Combine(Path.GetTempPath(), Path.GetFileName(Name));
            try
            {
                File.Delete(output);
            }
            catch {
            }
            int i = 1;
            while (File.Exists(output))
                output = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Name) + i++ + Path.GetExtension(Name));
            Base64Helper.DecodeToFile(Base64Data, output);
            return output;
        }
    }
}
