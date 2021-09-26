using BETarjetaCredito.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.IO;
using BETarjetaCredito.Util;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BETarjetaCredito.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarjetaController : ControllerBase
    {
        private readonly AplicationDbContext _context;

        //private static string _connStringSqlServer;

        /// <summary>
        /// recupera todas las tarjetas de creditos
        /// </summary>
        private const string QRY_SELECT_TARJETAS = "select Id, Titular, NumeroTarjeta, FechaExpiracion, CVV from TarjetaCredito"; 

        public TarjetaController(AplicationDbContext context)
        {
            _context = context;

            //_connStringSqlServer = config.GetConnectionString("DevConnection");
        }

        // GET: api/<TarjetaController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var listTarjetas = new List<TarjetaCredito>();
                //var constr = configuration.GetConnectionString("DevConnection");
                //var result = new DataSet();

                var datos = SqlServerUtil.ExecuteQueryDataSet(QRY_SELECT_TARJETAS);

                foreach (DataRow item in datos.Tables[0].Rows)
                {
                    listTarjetas.Add(new TarjetaCredito
                    {
                        Id = int.Parse(item["Id"].ToString()),
                        Titular = item["Titular"].ToString(),
                        NumeroTarjeta = item["NumeroTarjeta"].ToString(),
                        FechaExpiracion = item[3].ToString(),
                        CVV = item[4].ToString()
                    });
                }

                //var listTarjetas = await _context.TarjetaCredito.ToListAsync();
                return Ok(listTarjetas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<TarjetaController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TarjetaCredito tarjeta)
        {
            try
            {
                _context.Add(tarjeta);
                await _context.SaveChangesAsync();
                return Ok(tarjeta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<TarjetaController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TarjetaCredito tarjeta)
        {
            try
            {
                if (id != tarjeta.Id)
                {
                    return NotFound();
                }

                _context.Update(tarjeta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "La tarjeta fue actualizada con éxito!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        // DELETE api/<TarjetaController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tarjeta = await _context.TarjetaCredito.FindAsync(id);

                if (tarjeta == null)
                {
                    return NotFound();
                }

                _context.TarjetaCredito.Remove(tarjeta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "La tarjeta fue eliminada con éxito!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<TarjetaController>
        [HttpPost("saveFile")]
        public ActionResult SaveFile([FromForm] FileModel file)
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Files", file.FormFile.FileName);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    file.FormFile.CopyTo(stream);
                }

                return Ok(new { message = "La imagen fue agregada con éxito!", archivo = file.FormFile.FileName });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
