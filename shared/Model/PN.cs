namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    public bool givDosis(Dato givesDen) {
	    if (startDen <= givesDen.dato && givesDen.dato <= slutDen)
	    {
		    dates.Add(givesDen);
		    return true;
	    }
	    return false;
    }

    public override double doegnDosis()
    {
	    DateTime min = slutDen;
	    DateTime max = startDen;
	    foreach (Dato dosis in dates)
	    {
		    if (dosis.dato < min) 
			    min = dosis.dato;
		    if (dosis.dato > max)
			    max = dosis.dato;
	    }
	    int antaldage = (max - min).Days + 1;
	    double doegndosis = samletDosis()/antaldage;
	    return doegndosis;
    }


    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}
