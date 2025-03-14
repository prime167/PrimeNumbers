using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibPrime;

namespace PrimeTest;

public partial class Main : Form
{
    public Main()
    {
        InitializeComponent();
    }

    private async void bthOK_Click(object sender, EventArgs e)
    {
        var sw = new Stopwatch();
        bthOK.Enabled = false;
        var text = txtNumber.Text;
        BigInteger parsed;
        var b = BigInteger.TryParse(text, out parsed);
        if (b)
        {
            sw.Start();
            var certainty = int.Parse(numericUpDown1.Text);
            var rk = IsPrimeAsync(parsed, certainty, true);
            toolStripStatusLabel2.Text = "working";
            var r = await rk;
            sw.Stop();
            toolStripStatusLabel2.Text = $"{sw.Elapsed}";
            toolStripStatusLabel4.Text = txtNumber.Text.Length.ToString();
            bthOK.Enabled = true;
            sw.Reset();
            lblresult.Text = r ? "质数" : "不是质数";
        }
        else
        {
            lblresult.Text = "格式错误";
            bthOK.Enabled = true;
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        toolStripStatusLabel2.Text = TimeSpan.FromSeconds(0).ToString();
        comboBox1.SelectedIndex = 0;
        PollardRho_ p = new PollardRho_();
        BigInteger b = BigInteger.Parse("7396211860860335113117");
        BigInteger a;
        do
        {
            a = p.PollardRho(b);
            b = b / a;
            MessageBox.Show(a.ToString());
        } while (a > 1);
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        btnStart.Enabled = false;
        Cursor.Current = Cursors.WaitCursor;
        var digitCount = int.Parse(numericUpDown2.Text);
        var index = comboBox1.SelectedIndex;
        var start = "1" + new string('0', digitCount - 1);
        BigInteger s;
        s = BigInteger.Parse(start);
        if (index == 1)
        {

        }
        else
        {
            var min = s;
            var maxs = new string('9', digitCount);
            var max = BigInteger.Parse(maxs);
            s = Random(min, max);
        }

        if (s % 2 == 0)
        {
            s = s + 1;
        }

        var sw = new Stopwatch();
        sw.Start();
        for (var i = s; i < s * 2; i += 2)
        {
            if (i.IsProbablePrime())
            {
                txtNumber.Text = i.ToString();
                sw.Stop();
                toolStripStatusLabel2.Text = $"{sw.Elapsed}";
                btnStart.Enabled = true;
                sw.Reset();
                Cursor.Current = Cursors.Default;
                break;
            }
        }
    }

    // Generates a random BigInteger between min and max
    public BigInteger Random(BigInteger min, BigInteger max)
    {
        byte[] maxBytes = max.ToByteArray();
        BitArray maxBits = new BitArray(maxBytes);
        Random random = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < maxBits.Length; i++)
        {
            // Randomly set the bit
            int randomInt = random.Next();
            if ((randomInt % 2) == 0)
            {
                // Reverse the bit
                maxBits[i] = !maxBits[i];
            }
        }

        BigInteger result = new BigInteger();

        // Convert the bits back to a BigInteger
        for (int k = (maxBits.Count - 1); k >= 0; k--)
        {
            BigInteger bitValue = 0;

            if (maxBits[k])
            {
                bitValue = BigInteger.Pow(2, k);
            }

            result = BigInteger.Add(result, bitValue);
        }

        // Generate the random number
        BigInteger randomBigInt = BigInteger.ModPow(result, 1, BigInteger.Add(max, min));
        return randomBigInt;
    }

    private async Task<bool> IsPrimeAsync(BigInteger m, int certainty, bool alwasyUseRandom)
    {
        return await Task.Run(() => m.IsProbablePrime(certainty, alwasyUseRandom));
    }

    private static IEnumerable<BigInteger> Range(BigInteger fromInclusive, BigInteger toExclusive)
    {
        for (BigInteger i = fromInclusive; i < toExclusive; i++)
            yield return i;
    }

    public static void ParallelFor(BigInteger fromInclusive, BigInteger toExclusive, Action<BigInteger> body)
    {
        Parallel.ForEach(Range(fromInclusive, toExclusive), body);
    }
}