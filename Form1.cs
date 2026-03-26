namespace SimpleCalculator
{
    public partial class Form1 : Form
    {
        private string oprinput;
        private int txtresult;
        private int operand1;
        private int operand2;
        private int result;

        // 현재 입력중인 피연산자(문자열)와 전체 수식 표현
        private string _currentInput = string.Empty;
        private string _expression = string.Empty;
        // 결과가 표시된 상태인지 (이때는 입력 막음)
        private bool _isResultDisplayed = false;
        // last operation for repeat-equals behavior
        private string _lastOperator = string.Empty;
        private int _lastOperand2;
        private bool _hasLastOperation = false;
        // for repeated division as fraction: numerator fixed, denominator grows operand2^k
        private long _repeatNumerator = 0;
        private long _repeatDenominator = 1;
        private int _repeatCount = 0;


        public Form1()
        {
            InitializeComponent();
        }



        private void btnnum0_Click(object sender, EventArgs e)
        {
            AppendDigit("0");
        }

        private void btnnum1_Click(object sender, EventArgs e)
        {
            AppendDigit("1");
        }

        private void btnnum2_Click(object sender, EventArgs e)
        {
            AppendDigit("2");
        }

        private void btnnum3_Click(object sender, EventArgs e)
        {
            AppendDigit("3");
        }

        private void btnnum4_Click(object sender, EventArgs e)
        {
            AppendDigit("4");
        }

        private void btnnum5_Click(object sender, EventArgs e)
        {
            AppendDigit("5");
        }

        private void btnnum6_Click(object sender, EventArgs e)
        {
            AppendDigit("6");
        }

        private void btnnum7_Click(object sender, EventArgs e)
        {
            AppendDigit("7");
        }

        private void btnnum8_Click(object sender, EventArgs e)
        {
            AppendDigit("8");
        }

        private void btnnum9_Click(object sender, EventArgs e)
        {
            AppendDigit("9");
        }

        private void btnplus_Click(object sender, EventArgs e)
        {
            SetOperator("+");
        }

        private void btnminus_Click(object sender, EventArgs e)
        {
            SetOperator("-");
        }

        private void btnmultiplied_Click(object sender, EventArgs e)
        {
            SetOperator("*");
        }

        private void btndivied_Click(object sender, EventArgs e)
        {
            SetOperator("/");
        }

        // '=' 버튼: 두 피연산자의 정수 덧셈 수행(요구사항에 따라 Int 변환)
        private void btneq_Click(object sender, EventArgs e)
        {
            // If no operator set, but have a saved last operation and result displayed,
            // apply repeated-equals behavior: reapply lastOperator with lastOperand2
            if (string.IsNullOrEmpty(oprinput))
            {
                if (_hasLastOperation)
                {
                    // operand1 currently holds the last result
                    switch (_lastOperator)
                    {
                        case "+":
                            operand1 = operand1 + _lastOperand2;
                            break;
                        case "-":
                            operand1 = operand1 - _lastOperand2;
                            break;
                        case "*":
                            operand1 = operand1 * _lastOperand2;
                            break;
                        case "/":
                            if (_lastOperand2 == 0)
                                return;
                            // compute decimal expansion digits
                            var intPartRep = operand1 / _lastOperand2;
                            var remRep = Math.Abs(operand1 % _lastOperand2);
                            var decimalsRep = "";
                            var rem2 = remRep;
                            for (int i = 0; i < 10 && rem2 != 0; i++)
                            {
                                rem2 *= 10;
                                var digit = (int)(rem2 / _lastOperand2);
                                decimalsRep += digit.ToString();
                                rem2 = rem2 % _lastOperand2;
                            }
                            int upToRep = 0;
                            if (decimalsRep.Length == 0)
                                upToRep = 0;
                            else
                            {
                                var idx0 = decimalsRep.IndexOf('0');
                                if (idx0 == -1)
                                    upToRep = Math.Min(decimalsRep.Length, 5);
                                else
                                    upToRep = Math.Min(idx0, 5); // stop before first zero
                            }
                            string sRep;
                            if (upToRep == 0)
                                sRep = intPartRep.ToString();
                            else
                                sRep = intPartRep.ToString() + "." + decimalsRep.Substring(0, upToRep);

                            txtinput1.Text = operand1.ToString() + DisplayOperator(_lastOperator) + _lastOperand2.ToString() + "=" + sRep;
                            txtresult1.Text = sRep;
                            // update operand1 to integer approximation
                            try { operand1 = int.Parse(sRep.Split('.')[0]); } catch { }
                            _currentInput = operand1.ToString();
                            _expression = txtinput1.Text;
                            _isResultDisplayed = true;
                            return;
                        default:
                            return;
                    }

                    // For + - * repeated, update displays as integers
                    var exprRep = operand1.ToString() + _lastOperator + _lastOperand2.ToString() + "=" + operand1.ToString();
                    txtinput1.Text = exprRep;
                    txtresult1.Text = operand1.ToString();
                    _currentInput = operand1.ToString();
                    _expression = exprRep;
                    _isResultDisplayed = true;
                    return;
                }

                // no operator and no last operation: do nothing
                return;
            }

            // If current input is empty (operator was last input), show last operand as result
            if (string.IsNullOrEmpty(_currentInput))
            {
                result = operand1;
                var exprEmptyOperand = operand1.ToString() + DisplayOperator(oprinput) + "=" + result.ToString();
                txtinput1.Text = exprEmptyOperand;
                txtresult1.Text = result.ToString();
                _currentInput = result.ToString();
                _expression = exprEmptyOperand;
                oprinput = string.Empty;
                operand1 = result;
                _isResultDisplayed = true;
                // clear last operation
                _hasLastOperation = false;
                return;
            }

            // Parse second operand using int.Parse (입력은 숫자만 허용되어 있으므로 try-catch로 처리)
            try
            {
                operand2 = int.Parse(_currentInput);
            }
            catch
            {
                operand2 = 0;
            }

            // Perform operation
            switch (oprinput)
            {
                case "+":
                    result = operand1 + operand2;
                    break;
                case "-":
                    result = operand1 - operand2;
                    break;
                case "*":
                    result = operand1 * operand2;
                    break;
                case "/":
                    // division by zero: do nothing
                    if (operand2 == 0)
                        return;
                    // integer division with decimal trimming later; compute double result for display
                    result = operand1 / operand2;
                    break;
                default:
                    result = operand1 + operand2;
                    break;
            }

            // Display result. For division, show decimal up to 5 places trimmed; otherwise integer
            if (oprinput == "/")
            {
                var div = (double)operand1 / operand2;
                var s = div.ToString("F5");
                s = s.TrimEnd('0');
                if (s.EndsWith('.')) s = s.TrimEnd('.');
                var exprDiv = operand1.ToString() + DisplayOperator(oprinput) + operand2.ToString() + "=" + s;
                txtinput1.Text = exprDiv;
                txtresult1.Text = s;
                _currentInput = s;
                _expression = exprDiv;
            }
            else
            {
                var expr = operand1.ToString() + DisplayOperator(oprinput) + operand2.ToString() + "=" + result.ToString();
                txtinput1.Text = expr;
                txtresult1.Text = result.ToString();
                _currentInput = result.ToString();
                _expression = expr;
            }
            // save last operation for repeated '=' behavior
            var savedOp = oprinput;
            _lastOperator = savedOp;
            _lastOperand2 = operand2;
            _hasLastOperation = true;

            oprinput = string.Empty;
            operand1 = result;
            _isResultDisplayed = true;
            // reset repeat state
            _repeatCount = 0;
        }

        private void btnC_Click(object sender, EventArgs e)
        {
            // 전체 초기화
            _currentInput = string.Empty;
            _expression = string.Empty;
            oprinput = string.Empty;
            operand1 = operand2 = result = 0;
            txtinput1.Text = string.Empty;
            txtresult1.Text = string.Empty;
            _isResultDisplayed = false;
        }

        private void btnDEL_Click(object sender, EventArgs e)
        {
            // 현재 입력에서 마지막 문자 제거
            if (_isResultDisplayed)
            {
                // 결과 표시 중에는 DEL 동작하지 않음
                return;
            }

            if (!string.IsNullOrEmpty(_currentInput))
            {
                _currentInput = _currentInput[..^1];
                // expression에서도 마지막 문자 제거
                if (!string.IsNullOrEmpty(_expression))
                    _expression = _expression[..^1];
            }

            txtinput1.Text = _expression;
            txtresult1.Text = _currentInput;
        }

        private void btnCE_Click(object sender, EventArgs e)
        {
            // 현재 입력만 지우기(현재 입력 중인 피연산자)
            if (_isResultDisplayed)
            {
                // 결과 표시 중에는 CE 동작하지 않음
                return;
            }

            _expression = string.Empty;
            if (!string.IsNullOrEmpty(oprinput))
            {
                // expression에는 operand1와 연산자가 있어야 함
                _expression = operand1.ToString() + oprinput;
            }
            _currentInput = string.Empty;
            txtinput1.Text = _expression;
            txtresult1.Text = string.Empty;
        }

        private void btnpm_Click(object sender, EventArgs e)
        {
            // 부호 변경
            if (_isResultDisplayed)
            {
                // 결과 표시 중에는 +/- 동작하지 않음
                return;
            }

            try
            {
                var v = int.Parse(_currentInput);
                v = -v;
                _currentInput = v.ToString();
                // update expression: remove previous currentInput portion and append new
                // For simplicity, rebuild expression
                _expression = operand1 != 0 || !string.IsNullOrEmpty(oprinput) ? operand1.ToString() + oprinput + _currentInput : _currentInput;
                txtinput1.Text = _expression;
                txtresult1.Text = _currentInput;
            }
            catch
            {
                // parsing failed: do nothing
            }
        }

        // Helper: append digit to current input and expression, update textboxes
        private void AppendDigit(string d)
        {
            if (_isResultDisplayed)
                return; // 결과가 표시된 상태에서는 추가 입력 금지

            // 처음 입력 시 '0'이 들어가지 않도록 처리
            if (d == "0" && string.IsNullOrEmpty(_currentInput))
                return;

            _currentInput += d;
            _expression += d;
            txtinput1.Text = _expression;
            txtresult1.Text = _currentInput;
        }

        // Helper: set operator; store operand1 and append operator to expression
        private void SetOperator(string op)
        {
            if (_isResultDisplayed)
            {
                // allow operator after result: use displayed result as operand1
                _isResultDisplayed = false;
            }

            // If operator pressed first (no current input and empty expression),
            // treat first operand as 0 and show "0<op>" in input display.
            if (string.IsNullOrEmpty(_currentInput) && string.IsNullOrEmpty(_expression))
            {
                operand1 = 0;
                oprinput = op;
                _expression = "0" + DisplayOperator(op);
                txtinput1.Text = _expression;
                // keep current input empty, ready for second operand
                _currentInput = string.Empty;
                txtresult1.Text = string.Empty;
                return;
            }

            // Prevent duplicate operator: if operator already set and no current input, replace it
            if (!string.IsNullOrEmpty(oprinput) && string.IsNullOrEmpty(_currentInput))
            {
                // replace last operator in expression (use display symbol)
                if (!string.IsNullOrEmpty(_expression))
                    _expression = _expression[..^1] + DisplayOperator(op);
                oprinput = op;
                txtinput1.Text = _expression;
                return;
            }

            // 저장된 현재 입력값을 첫 번째 피연산자로 변환
            if (!string.IsNullOrEmpty(_currentInput))
            {
                // int.Parse 사용 (입력은 숫자만 허용되어 있으므로 안전하게 파싱)
                try
                {
                    operand1 = int.Parse(_currentInput);
                }
                catch
                {
                    operand1 = 0;
                }
            }

            oprinput = op;
            _expression += DisplayOperator(op);
            txtinput1.Text = _expression;
            // 현재 입력 초기화해서 두번째 피연산자 입력 준비
            _currentInput = string.Empty;
            txtresult1.Text = string.Empty;
        }

        // Map internal operator to display symbol
        private string DisplayOperator(string op)
        {
            return op switch
            {
                "*" => "×",
                "/" => "÷",
                _ => op,
            };
        }

        // 키보드 입력 처리: 숫자, +, Enter(=), Backspace
        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 허용: digits, '+', Enter (\r), Backspace handled in KeyDown
            if (char.IsDigit(e.KeyChar))
            {
                AppendDigit(e.KeyChar.ToString());
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '+')
            {
                SetOperator("+");
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '-')
            {
                SetOperator("-");
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '*')
            {
                SetOperator("*");
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '/')
            {
                SetOperator("/");
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '\r' || e.KeyChar == '=')
            {
                // Enter or '=' -> execute
                btneq_Click(this, EventArgs.Empty);
                e.Handled = true;
                return;
            }

            // ESC -> clear
            if ((int)e.KeyChar == 27)
            {
                btnC_Click(this, EventArgs.Empty);
                e.Handled = true;
                return;
            }

            // 차단: 문자 등 다른 입력은 무시
            e.Handled = true;
        }
    }
}
