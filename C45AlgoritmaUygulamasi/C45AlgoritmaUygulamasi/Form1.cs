using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C45AlgoritmaUygulamasi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Hasta> hastalar = HastalariGetir();

            List<int> yasBolumlemeListesi = new List<int> { 50, 60, 70 };
            List<int> yilBolumlemeListesi = new List<int> { 62, 63, 64 };
            List<int> dugumBolumlemeListesi = new List<int> { 5, 10, 19 };

            int durumBirSayisi = hastalar.Where(each => each.Durum == 1).Count(); //Linq //Durumu 1 olan kayıtların sayısı

            int durumIkiSayisi = hastalar.Where(each => each.Durum == 2).Count(); //Durumu 2 olan kayıtların sayısı

            int kayitSayisi = hastalar.Count(); //Bütün hastaların sayısı

            double durumBirOlasilik = Convert.ToDouble(durumBirSayisi) / Convert.ToDouble(kayitSayisi); //Durumu 1 olan kayıtların olasılık hesabı

            double durumIkiOlasilik = Convert.ToDouble(durumIkiSayisi) / Convert.ToDouble(kayitSayisi); //Durumu 2 olan kayıtların olasılık hesabı

            double H_Durum = EntropiHesapla(durumBirOlasilik, durumIkiOlasilik); //Entropi hesaplıyorum.

            decimal enBuyukYasKazanc = 0;
            double yasEnIyiBolumleme = 0;
            
            foreach (int item in yasBolumlemeListesi) //Hastanın yaşı için en iyi bölümleme hangisi onu seçiyoruz.
            {
                foreach (Hasta hasta in hastalar)
                {
                    if (Convert.ToInt32(hasta.Yas) >= item)
                    {
                        hasta.Yas = "B"; //Büyük eşit
                    }
                    else
                    {
                        hasta.Yas = "K"; //Kucuk
                    }
                }

                decimal yasKazanc = YasKazancHesaplama(hastalar, H_Durum); //Yaş için kazanç hesaplama işlemi yapar.

                if (yasKazanc > enBuyukYasKazanc)
                {
                    enBuyukYasKazanc = yasKazanc;
                    yasEnIyiBolumleme = item;
                }

                hastalar = HastalariGetir(); //Listeyi tekrar eski haline getirmek gerekiyor. Yoksa tekrar kontrol yaparken patlar.
            }

            decimal enBuyukYilKazanc = 0;
            double yilEnIyiBolumleme = 0;

            foreach (int item in yilBolumlemeListesi) //Hastanın operasyon(ameliyat) yılı için en iyi bölümleme hangisi onu seçiyoruz.
            {
                foreach (Hasta hasta in hastalar)
                {
                    if (Convert.ToInt32(hasta.OperasyonYili) >= item)
                    {
                        hasta.OperasyonYili = "B"; //Büyük eşit
                    }
                    else
                    {
                        hasta.OperasyonYili = "K"; //Kucuk
                    }
                }

                decimal yilKazanc = YilKazancHesaplama(hastalar, H_Durum);

                if (yilKazanc > enBuyukYilKazanc)
                {
                    enBuyukYilKazanc = yilKazanc;
                    yilEnIyiBolumleme = item;
                }

                hastalar = HastalariGetir();
            }

            decimal enBuyukDugumKazanc = 0;
            double dugumEnIyiBolumleme = 0;

            foreach (int item in dugumBolumlemeListesi)  //Hastanın düğüm sayısı (hastalık sayısı) için en iyi bölümleme hangisi onu seçiyoruz.
            {
                foreach (Hasta hasta in hastalar)
                {
                    if (Convert.ToInt32(hasta.DugumSayisi) >= item)
                    {
                        hasta.DugumSayisi = "B"; //Büyük eşit
                    }
                    else
                    {
                        hasta.DugumSayisi = "K"; //Kucuk
                    }
                }

                decimal dugumKazanc = DugumKazancHesaplama(hastalar, H_Durum);

                if (dugumKazanc > enBuyukDugumKazanc)
                {
                    enBuyukDugumKazanc = dugumKazanc;
                    dugumEnIyiBolumleme = item;
                }

                hastalar = HastalariGetir();
            }

            /*-----------------En iyi bölümlemelere göre Küçük/Büyük bölümlemesi yapılır.---------------------------*/

            foreach (Hasta hasta in hastalar)
            {
                if (Convert.ToInt32(hasta.Yas) >= yasEnIyiBolumleme)
                {
                    hasta.Yas = "B"; //Büyük eşit
                }
                else
                {
                    hasta.Yas = "K"; //Kucuk
                }

                if (Convert.ToInt32(hasta.OperasyonYili) >= yilEnIyiBolumleme)
                {
                    hasta.OperasyonYili = "B"; //Büyük eşit
                }
                else
                {
                    hasta.OperasyonYili = "K"; //Kucuk
                }

                if (Convert.ToInt32(hasta.DugumSayisi) >= dugumEnIyiBolumleme)
                {
                    hasta.DugumSayisi = "B"; //Büyük eşit
                }
                else
                {
                    hasta.DugumSayisi = "K"; //Kucuk
                }
            }

            /*-------------------------Kök düğümü(En tepedekini) hesapladık------------------------------------*/

            List<decimal> kazanclar = new List<decimal>
            {
                enBuyukYasKazanc, enBuyukYilKazanc, enBuyukDugumKazanc
            };

            decimal enBuyukKazanc = EnBuyukKazancBul(kazanclar);

            /*-------------------------Uç yaprakları hesaplayacağız------------------------------------*/

            List<string> bolumlemeler = new List<string> { "K", "B" };

            Sonuc sonuc = new Sonuc();

            Parallel.ForEach(bolumlemeler, bolumleme =>
            {
                if (enBuyukKazanc == enBuyukDugumKazanc)
                {
                    KokDugumSayisiIcinUcYaprakBul(hastalar, bolumleme, sonuc);
                }
                else if(enBuyukKazanc == enBuyukYasKazanc)
                {
                    KokYasIcinUcYaprakBul(hastalar, bolumleme, sonuc);
                }
                else if(enBuyukKazanc == enBuyukYilKazanc)
                {
                    KokOperasyonYiliIcinUcYaprakBul(hastalar, bolumleme, sonuc);
                }
            });

            lblUcYaprakSol.Text = sonuc.UcYaprakSol;
            lblUcYaprakSag.Text = sonuc.UcYaprakSag;
            lblKokDugum.Text = sonuc.KokDugum;
            lblUcSolDurumSol.Text = sonuc.UcYaprakSolDurumSol;
            lblUcSolDurumSag.Text = sonuc.UcYaprakSolDurumSag;
            lblUcSagDurumSol.Text = sonuc.UcYaprakSagDurumSol;
            lblUcSagDurumSag.Text = sonuc.UcYaprakSagDurumSag;
        }

        //Dosyadan özellikleri okur ve hasta listesini döner.
        public List<Hasta> HastalariGetir()
        {
            string pathInput = "../haberman.txt"; //Dosyayı proje altındaki bin klasörünün içine attım.

            FileStream fileStreamInput = new FileStream(pathInput, FileMode.Open, FileAccess.Read);

            StreamReader streamReaderInput = new StreamReader(fileStreamInput);

            string inputText = streamReaderInput.ReadLine(); //İlk satırı okuyor.

            List<Hasta> hastalar = new List<Hasta>(); //Boş bir hastalar listesi oluşturdum.

            while (inputText != null)
            {
                string[] ozellikler = inputText.Split(','); //İlk satırı ',' karakterine göre ayırıyorum.

                Hasta hasta = new Hasta(); //İlk satır özelliklerini oluşturduğum hasta modeline ekliyorum.
                hasta.Yas = ozellikler[0];
                hasta.OperasyonYili = ozellikler[1];
                hasta.DugumSayisi = ozellikler[2];
                hasta.Durum = Convert.ToInt32(ozellikler[3]);

                hastalar.Add(hasta);

                inputText = streamReaderInput.ReadLine(); //Bir sonraki satıra geçiyorum. Bir sonraki satır boş olana kadar while içinden çıkmaz.
            }

            streamReaderInput.Close();
            fileStreamInput.Close();

            return hastalar;
        }

        public decimal YasKazancHesaplama(List<Hasta> hastalar, double H_Durum)
        {
            int kayitSayisi = hastalar.Count;

            int yasKucukSayisi = hastalar.Where(each => each.Yas == "K").Count();

            int yasBuyukEsitSayisi = hastalar.Where(each => each.Yas == "B").Count();

            int yasBuyukEsitDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.Yas == "B").Count();

            int yasKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.Yas == "K").Count();

            int yasBuyukEsitDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.Yas == "B").Count();

            int yasKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.Yas == "K").Count();

            decimal yasKazanc = KazancHesapla(yasKucukDurumBirSayisi, yasKucukDurumIkiSayisi, yasKucukSayisi, yasBuyukEsitDurumBirSayisi, yasBuyukEsitDurumIkiSayisi, yasBuyukEsitSayisi, kayitSayisi, H_Durum);

            return yasKazanc;
        }

        public decimal YilKazancHesaplama(List<Hasta> hastalar, double H_Durum)
        {
            int kayitSayisi = hastalar.Count;

            int yilKucukSayisi = hastalar.Where(each => each.OperasyonYili == "K").Count();

            int yilBuyukEsitSayisi = hastalar.Where(each => each.OperasyonYili == "B").Count();

            int yilBuyukEsitDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.OperasyonYili == "B").Count();

            int yilKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.OperasyonYili == "K").Count();

            int yilBuyukEsitDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.OperasyonYili == "B").Count();

            int yilKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.OperasyonYili == "K").Count();

            decimal yilKazanc = KazancHesapla(yilKucukDurumBirSayisi, yilKucukDurumIkiSayisi, yilKucukSayisi, yilBuyukEsitDurumBirSayisi, yilBuyukEsitDurumIkiSayisi, yilBuyukEsitSayisi, kayitSayisi, H_Durum);

            return yilKazanc;
        }

        public decimal DugumKazancHesaplama(List<Hasta> hastalar, double H_Durum)
        {
            int kayitSayisi = hastalar.Count;

            int dugumKucukSayisi = hastalar.Where(each => each.DugumSayisi == "K").Count();

            int dugumBuyukEsitSayisi = hastalar.Where(each => each.DugumSayisi == "B").Count();

            int dugumBuyukEsitDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.DugumSayisi == "B").Count();

            int dugumKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.DugumSayisi == "K").Count();

            int dugumBuyukEsitDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.DugumSayisi == "B").Count();

            int dugumKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.DugumSayisi == "K").Count();

            decimal dugumKazanc = KazancHesapla(dugumKucukDurumBirSayisi, dugumKucukDurumIkiSayisi, dugumKucukSayisi, dugumBuyukEsitDurumBirSayisi, dugumBuyukEsitDurumIkiSayisi, dugumBuyukEsitSayisi, kayitSayisi, H_Durum);

            return dugumKazanc;
        }

        public decimal KazancHesapla(int kucukDurumBirSayisi, int kucukDurumIkiSayisi, int kucukSayisi, int buyukDurumBirSayisi, int buyukDurumIkiSayisi, int buyukSayisi, int kayitSayisi, double entropiDurum)
        {
            double yasKucukDurumBirOlasilik = Convert.ToDouble(kucukDurumBirSayisi) / Convert.ToDouble(kucukSayisi);

            double yasKucukDurumIkiOlasilik = Convert.ToDouble(kucukDurumIkiSayisi) / Convert.ToDouble(kucukSayisi);

            double H_Yas_Kucuk = EntropiHesapla(yasKucukDurumBirOlasilik, yasKucukDurumIkiOlasilik); //Entropi hesaplıyorum.

            double yasBuyukDurumBirOlasilik = Convert.ToDouble(buyukDurumBirSayisi) / Convert.ToDouble(buyukSayisi);

            double yasBuyukDurumIkiOlasilik = Convert.ToDouble(buyukDurumIkiSayisi) / Convert.ToDouble(buyukSayisi);

            double H_Yas_Buyuk = EntropiHesapla(yasBuyukDurumBirOlasilik, yasBuyukDurumIkiOlasilik); //Entropi hesaplıyorum. 

            double kucukOlasilik = Convert.ToDouble(kucukSayisi) / Convert.ToDouble(kayitSayisi);

            double buyukOlasilik = Convert.ToDouble(buyukSayisi) / Convert.ToDouble(kayitSayisi);

            double entropi = (kucukOlasilik * H_Yas_Kucuk) + (buyukOlasilik * H_Yas_Buyuk); // Kazanç hesaplama

            decimal kazanc = Convert.ToDecimal(entropiDurum) - Convert.ToDecimal(entropi);

            return kazanc;
        }       

        public double EntropiHesapla(double olasilik1, double olasilik2)
        {
            return ((olasilik1 * Math.Log(olasilik1, 2)) + (olasilik2 * Math.Log(olasilik2, 2))) * -1; //Entropi formülü
        }

        public decimal EnBuyukKazancBul(List<decimal> kazanclar)
        {
            decimal enBuyukKazanc = 0;

            foreach (decimal kazanc in kazanclar)
            {
                if (kazanc > enBuyukKazanc)
                {
                    enBuyukKazanc = kazanc;
                }
            }

            return enBuyukKazanc;
        }

        public Sonuc KokDugumSayisiIcinUcYaprakBul(List<Hasta> hastalar, string bolumleme, Sonuc sonuc)
        {
            sonuc.KokDugum = "Dugum Sayısı";

            int dugumBolumlemeSayisi = hastalar.Where(each => each.DugumSayisi == bolumleme).Count();
            int dugumIcinDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.DugumSayisi == bolumleme).Count();            
            int dugumIcinDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.DugumSayisi == bolumleme).Count();

            double dugumIcinDurumBirOlasilik = Convert.ToDouble(dugumIcinDurumBirSayisi) / Convert.ToDouble(dugumBolumlemeSayisi);
            double dugumIcinDurumIkiOlasilik = Convert.ToDouble(dugumIcinDurumIkiSayisi) / Convert.ToDouble(dugumBolumlemeSayisi);
            double H_Dugum_Entropi = EntropiHesapla(dugumIcinDurumBirOlasilik, dugumIcinDurumIkiOlasilik);

            /*---------------------------------------------------------------------------------------------------------------------------*/

            int dugumIcinYilKucukSayisi = hastalar.Where(each => each.DugumSayisi == bolumleme && each.OperasyonYili == "K").Count();
            int dugumIcinYilBuyukSayisi = hastalar.Where(each => each.DugumSayisi == bolumleme && each.OperasyonYili == "B").Count();
            int dugumIcinYilKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.DugumSayisi == bolumleme && each.OperasyonYili == "K").Count();
            int dugumIcinYilKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.DugumSayisi == bolumleme && each.OperasyonYili == "K").Count();
            int dugumIcinYilBuyukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.DugumSayisi == bolumleme && each.OperasyonYili == "B").Count();
            int dugumIcinYilBuyukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.DugumSayisi == bolumleme && each.OperasyonYili == "B").Count();

            decimal dugumIcinYilKazanc = KazancHesapla(dugumIcinYilKucukDurumBirSayisi, dugumIcinYilKucukDurumIkiSayisi, dugumIcinYilKucukSayisi, dugumIcinYilBuyukDurumBirSayisi, dugumIcinYilBuyukDurumIkiSayisi, dugumIcinYilBuyukSayisi, dugumBolumlemeSayisi, H_Dugum_Entropi);

            /*---------------------------------------------------------------------------------------------------------------------------*/

            int dugumIcinYasKucukSayisi = hastalar.Where(each => each.DugumSayisi == bolumleme && each.Yas == "K").Count();
            int dugumIcinYasBuyukSayisi = hastalar.Where(each => each.DugumSayisi == bolumleme && each.Yas == "B").Count();
            int dugumIcinYasKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.DugumSayisi == bolumleme && each.Yas == "K").Count();
            int dugumIcinYasKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.DugumSayisi == bolumleme && each.Yas == "K").Count();
            int dugumIcinYasBuyukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.DugumSayisi == bolumleme && each.Yas == "B").Count();
            int dugumIcinYasBuyukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.DugumSayisi == bolumleme && each.Yas == "B").Count();

            decimal dugumIcinYasKazanc = KazancHesapla(dugumIcinYasKucukDurumBirSayisi, dugumIcinYasKucukDurumIkiSayisi, dugumIcinYasKucukSayisi, dugumIcinYasBuyukDurumBirSayisi, dugumIcinYasBuyukDurumIkiSayisi, dugumIcinYasBuyukSayisi, dugumBolumlemeSayisi, H_Dugum_Entropi);

            if (dugumIcinYilKazanc > dugumIcinYasKazanc)
            {
                if (bolumleme == "K")
                {
                    sonuc.UcYaprakSol = "Yil";

                    SolUcYaprakIcinDurumBelirleme(dugumIcinYilKucukDurumBirSayisi, dugumIcinYilKucukDurumIkiSayisi, dugumIcinYilBuyukDurumBirSayisi, dugumIcinYilBuyukDurumIkiSayisi, sonuc);
                }
                else
                {
                    sonuc.UcYaprakSag = "Yil";

                    SagUcYaprakIcinDurumBelirleme(dugumIcinYilKucukDurumBirSayisi, dugumIcinYilKucukDurumIkiSayisi, dugumIcinYilBuyukDurumBirSayisi, dugumIcinYilBuyukDurumIkiSayisi, sonuc);
                }
            }
            else
            {
                if (bolumleme == "K")
                {
                    sonuc.UcYaprakSol = "Yas";

                    SolUcYaprakIcinDurumBelirleme(dugumIcinYasKucukDurumBirSayisi, dugumIcinYasKucukDurumIkiSayisi, dugumIcinYasBuyukDurumBirSayisi, dugumIcinYasBuyukDurumIkiSayisi, sonuc);
                }
                else
                {
                    sonuc.UcYaprakSag = "Yas";

                    SagUcYaprakIcinDurumBelirleme(dugumIcinYasKucukDurumBirSayisi, dugumIcinYasKucukDurumIkiSayisi, dugumIcinYasBuyukDurumBirSayisi, dugumIcinYasBuyukDurumIkiSayisi, sonuc);
                }
            }

            return sonuc;
        }

        public void KokYasIcinUcYaprakBul(List<Hasta> hastalar, string bolumleme, Sonuc sonuc)
        {
            sonuc.KokDugum = "Yas";

            int yasBolumlemeSayisi = hastalar.Where(each => each.Yas == bolumleme).Count();
            int yasIcinDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.Yas == bolumleme).Count();
            int yasIcinDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.Yas == bolumleme).Count();

            double yasIcinDurumBirOlasilik = Convert.ToDouble(yasIcinDurumBirSayisi) / Convert.ToDouble(yasBolumlemeSayisi);
            double yasIcinDurumIkiOlasilik = Convert.ToDouble(yasIcinDurumIkiSayisi) / Convert.ToDouble(yasBolumlemeSayisi);
            double H_Yas_Entropi = EntropiHesapla(yasIcinDurumBirOlasilik, yasIcinDurumIkiOlasilik);

            /*---------------------------------------------------------------------------------------------------------------------------*/

            int yasIcinYilKucukSayisi = hastalar.Where(each => each.Yas == bolumleme && each.OperasyonYili == "K").Count();
            int yasIcinYilBuyukSayisi = hastalar.Where(each => each.Yas == bolumleme && each.OperasyonYili == "B").Count();
            int yasIcinYilKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.Yas == bolumleme && each.OperasyonYili == "K").Count();
            int yasIcinYilKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.Yas == bolumleme && each.OperasyonYili == "K").Count();
            int yasIcinYilBuyukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.Yas == bolumleme && each.OperasyonYili == "B").Count();
            int yasIcinYilBuyukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.Yas == bolumleme && each.OperasyonYili == "B").Count();

            decimal yasIcinYilKazanc = KazancHesapla(yasIcinYilKucukDurumBirSayisi, yasIcinYilKucukDurumIkiSayisi, yasIcinYilKucukSayisi, yasIcinYilBuyukDurumBirSayisi, yasIcinYilBuyukDurumIkiSayisi, yasIcinYilBuyukSayisi, yasBolumlemeSayisi, H_Yas_Entropi);

            /*---------------------------------------------------------------------------------------------------------------------------*/

            int yasIcinDugumKucukSayisi = hastalar.Where(each => each.Yas == bolumleme && each.DugumSayisi == "K").Count();
            int yasIcinDugumBuyukSayisi = hastalar.Where(each => each.Yas == bolumleme && each.DugumSayisi == "B").Count();
            int yasIcinDugumKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.Yas == bolumleme && each.DugumSayisi == "K").Count();
            int yasIcinDugumKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.Yas == bolumleme && each.DugumSayisi == "K").Count();
            int yasIcinDugumBuyukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.Yas == bolumleme && each.DugumSayisi == "B").Count();
            int yasIcinDugumBuyukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.Yas == bolumleme && each.DugumSayisi == "B").Count();

            decimal yasIcinDugumKazanc = KazancHesapla(yasIcinDugumKucukDurumBirSayisi, yasIcinDugumKucukDurumIkiSayisi, yasIcinDugumKucukSayisi, yasIcinDugumBuyukDurumBirSayisi, yasIcinDugumBuyukDurumIkiSayisi, yasIcinDugumBuyukSayisi, yasBolumlemeSayisi, H_Yas_Entropi);

            if (yasIcinYilKazanc > yasIcinDugumKazanc)
            {
                if (bolumleme == "K")
                {
                    sonuc.UcYaprakSol = "Yil";
                    SolUcYaprakIcinDurumBelirleme(yasIcinYilKucukDurumBirSayisi, yasIcinYilKucukDurumIkiSayisi, yasIcinYilBuyukDurumBirSayisi, yasIcinYilBuyukDurumIkiSayisi, sonuc);
                }
                else
                {
                    sonuc.UcYaprakSag = "Yil";
                    SagUcYaprakIcinDurumBelirleme(yasIcinYilKucukDurumBirSayisi, yasIcinYilKucukDurumIkiSayisi, yasIcinYilBuyukDurumBirSayisi, yasIcinYilBuyukDurumIkiSayisi, sonuc);
                }
            }
            else
            {
                if (bolumleme == "K")
                {
                    sonuc.UcYaprakSol = "Dugum";
                    SolUcYaprakIcinDurumBelirleme(yasIcinDugumKucukDurumBirSayisi, yasIcinDugumKucukDurumIkiSayisi, yasIcinDugumBuyukDurumBirSayisi, yasIcinDugumBuyukDurumIkiSayisi, sonuc);
                }
                else
                {
                    sonuc.UcYaprakSag = "Dugum";
                    SagUcYaprakIcinDurumBelirleme(yasIcinDugumKucukDurumBirSayisi, yasIcinDugumKucukDurumIkiSayisi, yasIcinDugumBuyukDurumBirSayisi, yasIcinDugumBuyukDurumIkiSayisi, sonuc);
                }
            }
        }

        public Sonuc KokOperasyonYiliIcinUcYaprakBul(List<Hasta> hastalar, string bolumleme, Sonuc sonuc)
        {
            sonuc.KokDugum = "Operasyon Yılı";

            int yilBolumlemeSayisi = hastalar.Where(each => each.OperasyonYili == bolumleme).Count();
            int yilIcinDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.OperasyonYili == bolumleme).Count();            
            int yilIcinDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.OperasyonYili == bolumleme).Count();

            double yilIcinDurumBirOlasilik = Convert.ToDouble(yilIcinDurumBirSayisi) / Convert.ToDouble(yilBolumlemeSayisi);
            double yilIcinDurumIkiOlasilik = Convert.ToDouble(yilIcinDurumIkiSayisi) / Convert.ToDouble(yilBolumlemeSayisi);
            double H_Yil_Entropi = EntropiHesapla(yilIcinDurumBirOlasilik, yilIcinDurumIkiOlasilik);

            /*---------------------------------------------------------------------------------------------------------------------------*/

            int yilIcinDugumSayisiKucukSayisi = hastalar.Where(each => each.OperasyonYili == bolumleme && each.DugumSayisi == "K").Count();
            int yilIcinDugumSayisiBuyukSayisi = hastalar.Where(each => each.OperasyonYili == bolumleme && each.DugumSayisi == "B").Count();
            int yilIcinDugumSayisiKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.OperasyonYili == bolumleme && each.DugumSayisi == "K").Count();
            int yilIcinDugumSayisiKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.OperasyonYili == bolumleme && each.DugumSayisi == "K").Count();
            int yilIcinDugumSayisiBuyukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.OperasyonYili == bolumleme && each.DugumSayisi == "B").Count();
            int yilIcinDugumSayisiBuyukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.OperasyonYili == bolumleme && each.DugumSayisi == "B").Count();

            decimal yilIcinDugumKazanc = KazancHesapla(yilIcinDugumSayisiKucukDurumBirSayisi, yilIcinDugumSayisiKucukDurumIkiSayisi, yilIcinDugumSayisiKucukSayisi, yilIcinDugumSayisiBuyukDurumBirSayisi, yilIcinDugumSayisiBuyukDurumIkiSayisi, yilIcinDugumSayisiBuyukSayisi, yilBolumlemeSayisi, H_Yil_Entropi);

            /*---------------------------------------------------------------------------------------------------------------------------*/

            int yilIcinYasKucukSayisi = hastalar.Where(each => each.OperasyonYili == bolumleme && each.Yas == "K").Count();
            int yilIcinYasBuyukSayisi = hastalar.Where(each => each.OperasyonYili == bolumleme && each.Yas == "B").Count();
            int yilIcinYasKucukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.OperasyonYili == bolumleme && each.Yas == "K").Count();
            int yilIcinYasKucukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.OperasyonYili == bolumleme && each.Yas == "K").Count();
            int yilIcinYasBuyukDurumBirSayisi = hastalar.Where(each => each.Durum == 1 && each.OperasyonYili == bolumleme && each.Yas == "B").Count();
            int yilIcinYasBuyukDurumIkiSayisi = hastalar.Where(each => each.Durum == 2 && each.OperasyonYili == bolumleme && each.Yas == "B").Count();

            decimal yilIcinYasKazanc = KazancHesapla(yilIcinYasKucukDurumBirSayisi, yilIcinYasKucukDurumIkiSayisi, yilIcinYasKucukSayisi, yilIcinYasBuyukDurumBirSayisi, yilIcinYasBuyukDurumIkiSayisi, yilIcinYasBuyukSayisi, yilBolumlemeSayisi, H_Yil_Entropi);

            if (yilIcinDugumKazanc > yilIcinYasKazanc)
            {
                if (bolumleme == "K")
                {
                    sonuc.UcYaprakSol = "Dugum";
                    SolUcYaprakIcinDurumBelirleme(yilIcinDugumSayisiKucukDurumBirSayisi, yilIcinDugumSayisiKucukDurumIkiSayisi, yilIcinDugumSayisiBuyukDurumBirSayisi, yilIcinDugumSayisiBuyukDurumIkiSayisi, sonuc);
                }
                else
                {
                    sonuc.UcYaprakSag = "Dugum";
                    SagUcYaprakIcinDurumBelirleme(yilIcinDugumSayisiKucukDurumBirSayisi, yilIcinDugumSayisiKucukDurumIkiSayisi, yilIcinDugumSayisiBuyukDurumBirSayisi, yilIcinDugumSayisiBuyukDurumIkiSayisi, sonuc);
                }
            }
            else
            {
                if (bolumleme == "K")
                {
                    sonuc.UcYaprakSol = "Yas";
                    SolUcYaprakIcinDurumBelirleme(yilIcinYasKucukDurumBirSayisi, yilIcinYasKucukDurumIkiSayisi, yilIcinYasBuyukDurumBirSayisi, yilIcinYasBuyukDurumIkiSayisi, sonuc);
                }
                else
                {
                    sonuc.UcYaprakSag = "Yas";
                    SagUcYaprakIcinDurumBelirleme(yilIcinYasKucukDurumBirSayisi, yilIcinYasKucukDurumIkiSayisi, yilIcinYasBuyukDurumBirSayisi, yilIcinYasBuyukDurumIkiSayisi, sonuc);
                }
            }

            return sonuc;
        }

        public void SolUcYaprakIcinDurumBelirleme(int kucukDurumBirSayisi, int kucukDurumIkiSayisi, int buyukDurumBirSayisi, int buyukDurumIkiSayisi, Sonuc sonuc)
        {
            if (kucukDurumBirSayisi > kucukDurumIkiSayisi)
            {
                sonuc.UcYaprakSolDurumSol = "1";
            }
            else
            {
                sonuc.UcYaprakSolDurumSol = "2";
            }

            if (buyukDurumBirSayisi > buyukDurumIkiSayisi)
            {
                sonuc.UcYaprakSolDurumSag = "1";
            }
            else
            {
                sonuc.UcYaprakSolDurumSag = "2";
            }
        }

        public void SagUcYaprakIcinDurumBelirleme(int kucukDurumBirSayisi, int kucukDurumIkiSayisi, int buyukDurumBirSayisi, int buyukDurumIkiSayisi, Sonuc sonuc)
        {
            if (kucukDurumBirSayisi > kucukDurumIkiSayisi)
            {
                sonuc.UcYaprakSagDurumSol = "1";
            }
            else
            {
                sonuc.UcYaprakSagDurumSol = "2";
            }

            if (buyukDurumBirSayisi > buyukDurumIkiSayisi)
            {
                sonuc.UcYaprakSagDurumSag = "1";
            }
            else
            {
                sonuc.UcYaprakSagDurumSag = "2";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
