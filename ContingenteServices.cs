using SDA.DAL.Data;
using SDA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.Services
{
    public class ContingenteServices
    {

        public static Double GetMontoAsignado(int detalleContingenteId)
        {
            Double totalAsignado = 0.00;
            DataContext db = new DataContext();
            var summ = from d in db.DetallesContingente
                       join s in db.Solicitudes on d.detalleContingenteId equals s.detalleContingenteId
                       where d.detalleContingenteId == detalleContingenteId
                       group new { s.volumenAsignado }
                       by new { d.detalleContingenteId } into grp
                       select new
                       {
                           Asignado = grp.Sum(s => s.volumenAsignado)
                       };
            //
            foreach (var summary in summ)
            {
                totalAsignado = (Double)summary.Asignado;
            }
            
            return totalAsignado;
        }



        public static Double GetMontoImportado(int detalleContingenteId)
        {
            Double total = 0.00;
            DataContext db = new DataContext();
            var summ = from d in db.DetallesContingente
                       join s in db.Solicitudes on d.detalleContingenteId equals s.detalleContingenteId
                       where d.detalleContingenteId == detalleContingenteId
                       group new { s.volumenAsignado, s.volumenSolicitado, s.volumenImportado }
                       by new { d.detalleContingenteId } into grp
                       select new
                       {
                           Asignado = grp.Sum(s => s.volumenAsignado),
                           Solicitado = grp.Sum(s => s.volumenSolicitado),
                           Importado = grp.Sum(s => s.volumenImportado),
                           DetalleContingenteId = grp.Key.detalleContingenteId
                       };
            //
            foreach (var summary in summ)
            {
                total = (Double)summary.Importado;
            }

            return total;
        }

        //public static Double GetCantidadSolicitudes(int detalleContingenteId, bool historico)
        //{
        //    DataContext db = new DataContext();
        //    var counter = (from d in db.DetallesContingente
        //                   join s in db.Solicitudes on d.detalleContingenteId equals s.detalleContingenteId
        //                   where d.detalleContingenteId == detalleContingenteId
        //                   && s.retirarReasignacion == false 
        //                   && ((s.esImportadorHistorico == "Y" && historico) || (!historico))).Count();
        //    //
        //    return counter;
        //}

        public static void Reasignar(int detalleContingenteId, bool historico)
        {
            DataContext db = new DataContext();
            var cmd = db.GetConnection().CreateCommand();
            cmd.CommandText = "[dbo].[proc_Redistribuir] " + detalleContingenteId.ToString() + ", " + (historico ? "'Y'" : "'N'");
            cmd.ExecuteNonQuery();
        }

        public static void Asignar(int detalleContingenteId, bool historico)
        {
            DataContext db = new DataContext();
            var cmd = db.GetConnection().CreateCommand();
            cmd.CommandText = "[dbo].[proc_Distribuir] " + detalleContingenteId.ToString() + ", " + (historico ? "'Y'" : "'N'");
            cmd.ExecuteNonQuery();
        }

        public static void Asignar(int year)
        {
            DataContext db = new DataContext();
            var model = (from t in db.DetallesContingente
                         where t.anio == year
                         select t).ToList();
            var cmd = db.GetConnection().CreateCommand();
            //
            db.GetConnection().Open();
            foreach (DetalleContingente contingente in model)
            {
                cmd.CommandText = "[dbo].[proc_Distribuir] " + contingente.detalleContingenteId.ToString() + ", 'Y'";
                cmd.ExecuteNonQuery();
                //db.GetConnection().Close();
                //
                cmd.CommandText = "[dbo].[proc_Distribuir] " + contingente.detalleContingenteId.ToString() + ", 'N'";
                //db.GetConnection().Open();
                cmd.ExecuteNonQuery();
                //db.GetConnection().Close();

                cmd.CommandText = "[dbo].[proc_Distribuir] " + contingente.detalleContingenteId.ToString() + ", 'I'";
                //db.GetConnection().Open();
                cmd.ExecuteNonQuery();
            }
            db.GetConnection().Close();

        }

        public static void Reasignar(int year)
        {
            DataContext db = new DataContext();
            var model = (from t in db.DetallesContingente
                         where t.anio == year
                         select t).ToList();
            var cmd = db.GetConnection().CreateCommand();
            //
            db.GetConnection().Open();
            foreach (DetalleContingente contingente in model)
            {                
                cmd.CommandText = "[dbo].[proc_Redistribuir] " + contingente.detalleContingenteId.ToString() + ", 'Y'";
                cmd.ExecuteNonQuery();
                //db.GetConnection().Close();
                //
                cmd.CommandText = "[dbo].[proc_Redistribuir] " + contingente.detalleContingenteId.ToString() + ", 'N'";
                //db.GetConnection().Open();
                cmd.ExecuteNonQuery();
                //db.GetConnection().Close();

                cmd.CommandText = "[dbo].[proc_Redistribuir] " + contingente.detalleContingenteId.ToString() + ", 'I'";
                //db.GetConnection().Open();
                cmd.ExecuteNonQuery();
            }
            db.GetConnection().Close();

        }

        //        public static Double Reasignar(int detalleContingenteId, bool historico)
        //        {
        //            DataContext db = new DataContext();
        //            Double totalReasignacion = 0.00;
        //            int cantidadSolicitudes = 0;
        //            using (var cmd = db.GetConnection().CreateCommand())
        //            {
        //                cmd.CommandText = "select sum(case when((volumenAsignado - volumenImportado) > volumenSolicitadoReasignacion and retirarReasignacion = 0 " +
        //                    " and volumenSolicitadoReasignacion > 0.00) then     (volumenAsignado - volumenImportado - volumenSolicitadoReasignacion) " +
        //                    " when retirarReasignacion = 1 then(volumenAsignado - volumenImportado) else 0.00 end) Redistribuir " +
        //                    " from Solicitud WHERE s.detalleContingenteId = " + detalleContingenteId.ToString();
        //                var results = cmd.ExecuteReader();
        //                foreach (Double res in results)
        //                {
        //                    totalReasignacion += res;
        //                }

        //            }

        //            select
        //sum(case when((volumenAsignado - volumenImportado) > volumenSolicitadoReasignacion and retirarReasignacion = 0
        //and volumenSolicitadoReasignacion > 0.00) then
        //    (volumenAsignado - volumenImportado - volumenSolicitadoReasignacion)

        //    when retirarReasignacion = 1 then(volumenAsignado - volumenImportado) else 0.00 end) Redistribuir
        // from Solicitud

        //             var counter = (from d in db.DetallesContingente
        //                           join s in db.Solicitudes on d.detalleContingenteId equals s.detalleContingenteId
        //                           where d.detalleContingenteId == detalleContingenteId
        //                           && s.retirarReasignacion == false
        //                           && ((s.esImportadorHistorico == "Y" && historico) || (!historico))).Count();
        //            //
        //            return counter;
        //        }

        public static int CountSolicitudesReasignacion()
        {
            DataContext db = new DataContext();
            var data = db.Solicitudes.Where(d => d.volumenReasignacion != (decimal)0.00).Count();
            return (int)data;
        }

        public static Double SumSolicitudesReasignacion()
        {
            DataContext db = new DataContext();
            var data = db.Solicitudes;
            var suma = data.Sum(d => d.volumenReasignacion);
            return (Double)suma;
        }

        public static Double SumDisponibleReasignacion(int detalleContingente)
        {
            Double disponible = 0.00;
            //
            using (var context = new DataContext())
            {
                var disponibles = context.Solicitudes.SqlQuery("SELECT [dbo].[fnc_getDisponibleRedistribucion] (" +
                    detalleContingente.ToString() + ")").ToList();
                //disponible = disponibles[0].ToString();
            }
            return disponible;
        }
    }
}
