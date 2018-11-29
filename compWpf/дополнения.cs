using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace compWpf
{
    public partial class виды
    {
        public int дистанций => дистанции.Count(n => n.слет == клСлет.слет);

        public override string ToString() => наимен;

    }
    public partial class слеты
    {
        public int дистанций => дистанции.Count;
        public int туристов  => туристы.Count;
        public override string ToString() => наимен;

    }

    public partial class школы
    {
        public int экипажей => экипажи.Count(n=>n.дистанции.слет==клСлет.слет);
        public int туристов => туристы.Count(n=>n.слет == клСлет.слет);
        public override string ToString() => наимен;
      

    }

    //public partial class участники
    //{

    //    //  public override string ToString() => "участник";

    //}

    public partial class туристы
    {
        public int экипажей
        {
            get
            {
                int tt = 0;
                if (экипажи != null)
                {
                    tt = экипажи.Count();
                }
                return tt;
            }
        }
          //  => экипажи.Count(n=>n.дистанции.слет==клСлет.слет);
      //  public int слетов => участники.Count;
        public override string ToString() => фамилия.Trim() + " " + имя.Trim();
        public bool выбран { get; set; } = false;

        //  public bool участник => участники.Any(n => n.слет == клСлет.слет);
        //  public bool участник { get; set; } = false;
        //public int номер => экипажи.Max(n => n.номер);

    }
    public partial class дистанции
    {
           public int экипажей => экипажи.Count;
     //   public int судов => суда.Count;     
        public int этапов => этапы.Count;
        public override string ToString() => наимен;
        //// public bool установлена { get; set; } = false;
        //  public bool установлена => трассы.Any(n => n.слет == клСлет.слет);


    }

    public partial class экипажи
    {
        public int матросов => туристы.Count;
        public override string ToString() => номер.ToString() + " " + школы.ToString()+" "+прим;
        //    public string состав { get; set; }
        //  public int лутший { get; set; } = 0;
        public string состав => состав_экипажа();
        string состав_экипажа()
        {
            string ss = "";
            int j = 0;
            foreach (туристы mRow in туристы)
            {
                if (mRow != null)
                {
                    j++;
                    if (j == матросов)
                    {
                        ss += mRow.фамилия.Trim() + " " + mRow.имя;
                    }
                    else
                    {
                        ss += mRow.фамилия.Trim() + " " + mRow.имя + System.Environment.NewLine;
                    }
                }
            }
            return ss;
        }
        public int попыток => результаты.Count(n => n.итог > 0);

    }


    public partial class результаты
    {
        [Range(0, 59, ErrorMessage ="Введите секунды от 0 до 59")]
        public int провСек => время_сек;
      
        public int номер
        {
            get
            {
                int nn = 0;
                if(экипажи != null)
                {
                    nn= экипажи.номер;
                }
                return nn;
            }
        }
            //=> экипажи.номер;
        public string клуб
        {
            get
            {
                string ss = "";
                if(экипажи != null )
                {
                    ss = экипажи.школы.ToString();
                }
                return ss;
            }
        }
            
            //=> экипажи.школы.ToString();
        //    public string состав => экипажи.состав;
        public int место
        {
            get
            {
                int nn = 0;
                if (экипажи != null)
                {
                    nn = экипажи.место;
                }
                return nn;
            }
        }
//=> экипажи.место;
        public int лучший
        {
            get
            {
                int nn = 0;
                if (экипажи != null)
                {
                    nn = экипажи.итог;
                }
                return nn;
            }
        }
        //=> экипажи.итог;
        public string состав
        {
            get
            {
                string ss = "";
                if (экипажи != null)
                {
                    ss = экипажи.состав;
                }
                return ss;
            }
        }

        //=> экипажи.состав;

        public int штрафов   => штрафы.Count;


        public string наимен_дистанции
        {
            get
            {
                string ss = "";
                if(экипажи.дистанции != null )
                {
                    ss = экипажи.дистанции.наимен;
                }
                return ss;
            }
        }
        public string наимен_судна
        {
            get
            {
                string ss = "";
                if(экипажи != null)
                {
                    if (экипажи.суда != null)
                    {
                        ss = экипажи.суда.наимен;
                    }
                }
                return ss;
            }

        }
            
        
        public string наимен_школы
        {
            get
            {
                string ss = "";
                if (экипажи != null)
                {
                    if (экипажи.школы != null)
                    {
                        ss = экипажи.школы.наимен;
                    }
                }
                return ss;
            }
        }
        public int сумма_штрафов
        {
            get
            {
                int ss = 0;
                if (штрафы.Any())
                {
                    ss = штрафы.Sum(n => n.секунд);
                }
                return ss;
            }
        }
        //    public int набежало { get; set; }
        public DateTime старт { get; set; } = DateTime.Today;
        public DateTime финиш { get; set; } = DateTime.Today;
        public bool зачетный { get; set; } = false;
        public bool плывут { get; set; } = false;
        public int миллисекунд { get; set; } = 0;
        public string старт_финиш
        {
            get
            {
                string ss = "Старт";
                if (плывут)
                {
                    ss = "Финиш " + миллисекунд.ToString();
                }
                return ss;
            }
        }

        public bool забег { get; set; } = false;
        public string готов
        {
            get
            {
                string ss = "Очередь";
                if (итог>0)
                {
                    ss = "Причалил";
                }
                if (забег)
                {
                    ss = "Готов" ;
                }
                if (плывут)
                {
                    ss = "Плывут";
                }
                return ss;
            }
        }
        //public int число_мин
        //{
        //    set
        //    {
        //        время_мин = value;
        //        if (Moving != null)
        //        {
        //            поле = "время_мин";
        //            Moving(this);
        //        }
        //    }
        //    get { return время_мин; }
        //}
        // public string поле = "";
        //public static event Action<результаты> Moving;
    }



    public partial class этапы
    {
        public int штрафов => штрафы.Count;
        public override string ToString() => наимен;
    }
    public partial class суда
    {
        public int экипажей => экипажи.Count;
        public override string ToString() => наимен;
    }

}
