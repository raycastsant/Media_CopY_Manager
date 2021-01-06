using MCP.db;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP.gui.components
{
    /**Se utiliza esta clase para evitar consultar las colecciones 
     * del DBContext en los Threads*/
    public class MediaFile_Basic_Info
    {
        private double costo;
        //private long size;

        public int id { get; set; }
        public int punto_copia_id { get; set; }
        public string titulo { get; set; }
        public int tipo_categoria_id { get; set; }
        public string nombre_categoria { get; set; }
        public string file_url { get; set; }
        public string str_file { get; set; }
        public string fichero_trailer { get; set; }
        public string fichero_portada { get; set; }
        public bool is_folder { get; set; }
        public bool file_exists { get; set; }

        public FileSystemInfo _fileSystemInfo { get; set; }

        public List<MediaFile_Basic_Info> Childrens { get; set; }

        public MediaFile_Basic_Info()
        {
            Childrens = new List<MediaFile_Basic_Info>();
            is_folder = false;
            costo = 0;
        }

        public void ReadMediaFile(media_files mf)
        {
            id = mf.id;
            punto_copia_id = mf.punto_copia_id;
            titulo = mf.titulo;
            nombre_categoria = mf.categoria.categoria1;
            file_url = mf.file_url;
            str_file = mf.str_file;
            fichero_trailer = mf.fichero_trailer;
            fichero_portada = mf.fichero_portada;
            is_folder = mf.is_folder;
            tipo_categoria_id = mf.categoria.tipo_categoria_id;
            
            if (is_folder)
            {
                ReloadChildrens(mf.categoria_id);
            }
        }

        private void ReloadChildrens(int categoria_id)
        {
            if (this.is_folder)
            {
                List<media_files> mflist = DBManager.MediaFilesRepo.FindByCategoria(categoria_id, this.id, false);
                MediaFile_Basic_Info child;
                foreach (media_files mf in mflist)
                {
                    child = new MediaFile_Basic_Info();
                    child.ReadMediaFile(mf);
                    this.Childrens.Add(child);
                }
            }
        }

        public double getCosto(int tipoPagoId)
        {
            costo = 0;
            double coef = 0;

            coeficientes_pago cp = DBManager.CoeficientesRepo.FindCosto(this.tipo_categoria_id, tipoPagoId);
            if (cp != null)
                coef = cp.valor;

            if (is_folder)
            {
                updateCostFromChildren(Childrens, coef);
            }
            else
                costo = coef;

            return costo;
        }

        private void updateCostFromChildren(List<MediaFile_Basic_Info> childList, double coeficiente)
        {
            foreach (MediaFile_Basic_Info mfbi in childList)
            {
                if (!mfbi.is_folder)
                    costo += coeficiente;
                else
                {
                    updateCostFromChildren(mfbi.Childrens, coeficiente);
                }
            }
        }

        public long getTotalSize()
        {
            return calculate_size(this);
        }

        private long calculate_size(MediaFile_Basic_Info mfbi)
        {
            long size = 0;
            if (mfbi.is_folder)
            {
                foreach (MediaFile_Basic_Info item in mfbi.Childrens)
                {
                    if (mfbi.is_folder)
                        size += calculate_size(item);
                }
            }
            else
            {
                FileInfo fi = new FileInfo(mfbi.file_url);
                size += fi.Length;
            }

            return size;
        }

        public override string ToString()
        {
            return titulo + " (" + nombre_categoria + ")";
        }
    }
}
