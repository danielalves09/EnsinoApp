using System;
using System.Collections.Generic;

namespace EnsinoApp.ViewModels.Home;

public class DashboardViewModel
{

    public int TotalCasais { get; set; }
    public int TotalInscricoes { get; set; }
    public int MatriculasAtivas { get; set; }
    public int TurmasAtivas { get; set; }


    public List<InscricaoPorMes> InscricoesPorMes { get; set; } = new List<InscricaoPorMes>();


    public List<MatriculasPorCurso> MatriculasPorCurso { get; set; } = new List<MatriculasPorCurso>();


    public List<CasaisPorCampus> CasaisPorCampus { get; set; } = new List<CasaisPorCampus>();
}

public class InscricaoPorMes
{
    public string MesAno { get; set; } = null!;
    public int Total { get; set; }
}

public class MatriculasPorCurso
{
    public string Curso { get; set; } = null!;
    public int Total { get; set; }
}

public class CasaisPorCampus
{
    public string Campus { get; set; } = null!;
    public int Total { get; set; }
}

