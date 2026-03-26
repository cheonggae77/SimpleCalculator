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
        private bool _leadingZeroShown = false;
        private bool _isError = false;


        public Form1()
        {
            InitializeComponent();
        }

        // Evaluate arithmetic expression in _expression (display form with × and ÷ allowed)
        // Returns (success, value). On divide-by-zero returns (false, 0) and sets error state.
        private (bool ok, int value) EvaluateExpression(string exprDisplay)
        {
            if (string.IsNullOrWhiteSpace(exprDisplay))
                return (false, 0);

            // normalize display operators to internal
            var expr = exprDisplay.Replace('×', '*').Replace('÷', '/');
            // remove any trailing = or result part
            var eqIndex = expr.IndexOf('=');
            if (eqIndex >= 0)
                expr = expr.Substring(0, eqIndex);

            // Tokenize (handle unary minus)
            var tokens = new List<string>();
            for (int i = 0; i < expr.Length; i++)
            {
                var c = expr[i];
                if (char.IsWhiteSpace(c))
                    continue;
                if (char.IsDigit(c))
                {
                    var j = i;
                    while (j < expr.Length && char.IsDigit(expr[j])) j++;
                    tokens.Add(expr.Substring(i, j - i));
                    i = j - 1;
                    continue;
                }
                if (c == '+' || c == '*' || c == '/' )
                {
                    tokens.Add(c.ToString());
                    continue;
                }
                if (c == '-')
                {
                    // unary if at start or after '(' or other operator
                    if (tokens.Count == 0 || tokens.Last() == "(" || tokens.Last() == "+" || tokens.Last() == "-" || tokens.Last() == "*" || tokens.Last() == "/")
                    {
                        // treat unary minus as 0 - number: push 0 and '-'
                        tokens.Add("0");
                        tokens.Add("-");
                    }
                    else
                    {
                        tokens.Add("-");
                    }
                    continue;
                }
                if (c == '(' || c == ')')
                {
                    tokens.Add(c.ToString());
                    continue;
                }
                // unknown char -> fail
                return (false, 0);
            }

            // Shunting-yard to RPN
            var output = new List<string>();
            var ops = new Stack<string>();
            int Prec(string o) => o == "+" || o == "-" ? 1 : 2;
            foreach (var tk in tokens)
            {
                if (int.TryParse(tk, out _))
                {
                    output.Add(tk);
                    continue;
                }
                if (tk == "+" || tk == "-" || tk == "*" || tk == "/")
                {
                    while (ops.Count > 0 && ops.Peek() != "(" && (Prec(ops.Peek()) >= Prec(tk)))
                    {
                        output.Add(ops.Pop());
                    }
                    ops.Push(tk);
                    continue;
                }
                if (tk == "(")
                {
                    ops.Push(tk);
                    continue;
                }
                if (tk == ")")
                {
                    while (ops.Count > 0 && ops.Peek() != "(")
                        output.Add(ops.Pop());
                    if (ops.Count == 0) return (false, 0); // mismatched paren
                    ops.Pop(); // pop '('
                    continue;
                }
            }
            while (ops.Count > 0)
            {
                var o = ops.Pop();
                if (o == "(" || o == ")") return (false, 0);
                output.Add(o);
            }

            // Evaluate RPN
            var st = new Stack<long>();
            foreach (var tk in output)
            {
                if (int.TryParse(tk, out var n))
                {
                    st.Push(n);
                    continue;
                }
                if (st.Count < 2) return (false, 0);
                var b = st.Pop();
                var a = st.Pop();
                long r = 0;
                switch (tk)
                {
                    case "+": r = a + b; break;
                    case "-": r = a - b; break;
                    case "*": r = a * b; break;
                    case "/":
                        if (b == 0)
                            return (false, 0);
                        r = a / b; break;
                    default: return (false, 0);
                }
                st.Push(r);
            }
            if (st.Count != 1) return (false, 0);
            var final = (int)st.Pop();
            return (true, final);
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
            // If an expression has been built (supports multiple operators and parentheses),
            // evaluate the full expression using the expression evaluator so things like
            // "8*(3+4)" work correctly. The expression is stored in `_expression` (display
            // form, may contain ×/÷). If no full expression is present, try to build one
            // from operand1/oprinput/_currentInput. Fall back to repeated-equals behavior
            // only if nothing else is available.
            string exprDisplay = null;

            if (!string.IsNullOrEmpty(_expression))
            {
                // If _expression already contains a previous result like "...=...", strip it
                var idx = _expression.IndexOf('=');
                exprDisplay = idx >= 0 ? _expression.Substring(0, idx) : _expression;
            }
            else if (!string.IsNullOrEmpty(_currentInput) && !string.IsNullOrEmpty(oprinput))
            {
                exprDisplay = operand1.ToString() + DisplayOperator(oprinput) + _currentInput;
            }
            else if (!string.IsNullOrEmpty(_currentInput))
            {
                exprDisplay = _currentInput;
            }

            if (string.IsNullOrEmpty(exprDisplay))
            {
                // No expression to evaluate; if we have a saved last operation, apply repeated-equals
                if (_hasLastOperation)
                {
                    switch (_lastOperator)
                    {
                        case "+": operand1 = operand1 + _lastOperand2; break;
                        case "-": operand1 = operand1 - _lastOperand2; break;
                        case "*": operand1 = operand1 * _lastOperand2; break;
                        case "/":
                            if (_lastOperand2 == 0)
                                return;
                            operand1 = operand1 / _lastOperand2; break;
                        default: return;
                    }

                    var exprRep = operand1.ToString() + DisplayOperator(_lastOperator) + _lastOperand2.ToString() + "=" + operand1.ToString();
                    txtinput1.Text = exprRep;
                    txtresult1.Text = operand1.ToString();
                    _currentInput = operand1.ToString();
                    _expression = exprRep;
                    _isResultDisplayed = true;
                }
                return;
            }

            // Evaluate the constructed expression
            var (ok, val) = EvaluateExpression(exprDisplay);
            if (!ok)
            {
                txtresult1.Text = "0으로 나눌수 없습니다.";
                txtresult1.ForeColor = System.Drawing.Color.Red;
                _isError = true;
                return;
            }

            // Show full expression with result in input box, and only the numeric result in result box
            txtinput1.Text = exprDisplay + "=" + val.ToString();
            txtresult1.Text = val.ToString();
            txtresult1.ForeColor = val < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;

            // Update internal state
            _expression = txtinput1.Text;
            _currentInput = val.ToString();
            oprinput = string.Empty;
            operand1 = val;
            _isResultDisplayed = true;
            _hasLastOperation = false;
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
            txtresult1.ForeColor = System.Drawing.Color.Black;
            _leadingZeroShown = false;
            _isError = false;
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
                // expression에는 operand1와 연산자가 있어야 함 (표시 기호로 변환)
                _expression = operand1.ToString() + DisplayOperator(oprinput);
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

            // If an error message is shown, clear it when user starts typing a digit
            if (_isError)
            {
                btnC_Click(this, EventArgs.Empty);
                // continue to process the digit
            }

            // 처음 입력 시 '0'이 들어가는 처리: txtresult1에는 한 번만 0 표시, txtinput1에는 추가하지 않음
            if (string.IsNullOrEmpty(_currentInput) && d == "0")
            {
                if (!_leadingZeroShown)
                {
                    txtresult1.Text = "0";
                    _leadingZeroShown = true;
                }
                return;
            }

            // 첫 입력이 0으로 이미 표시되어 있고 사용자가 0이 아닌 숫자를 입력하면
            // txtresult1의 0을 제거하고 실제 숫자로 교체, txtinput1에도 추가
            if (string.IsNullOrEmpty(_currentInput) && _leadingZeroShown && d != "0")
            {
                _currentInput = d;
                _expression += d;
                txtinput1.Text = _expression;
                txtresult1.Text = _currentInput;
                _leadingZeroShown = false;
                return;
            }

            _currentInput += d;
            _expression += d;
            txtinput1.Text = _expression;
            txtresult1.Text = _currentInput;
        }

        // Append a parenthesis to expression and show position in txtinput1
        private void AppendParen(char p)
        {
            // If currently showing result, reset to allow new expression
            if (_isResultDisplayed)
            {
                _currentInput = string.Empty;
                _expression = string.Empty;
                _isResultDisplayed = false;
            }

            _expression += p;
            txtinput1.Text = _expression;
            // show paren in result box as well for visibility
            txtresult1.Text = p.ToString();
        }

        // Helper: set operator; store operand1 and append operator to expression
        private void SetOperator(string op)
        {
            if (_isResultDisplayed)
            {
                // allow operator after result: use displayed result as operand1
                _isResultDisplayed = false;
            }

            // clear any leading zero shown when operator pressed
            _leadingZeroShown = false;

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
                // show the first operand (0) in result box
                txtresult1.Text = operand1.ToString();
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
            // keep showing last entered operand in txtresult1
            txtresult1.Text = operand1.ToString();
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

            // Parentheses input from keyboard
            if (e.KeyChar == '(' || e.KeyChar == ')')
            {
                AppendParen(e.KeyChar);
                e.Handled = true;
                return;
            }

            if (e.KeyChar == '\r' || e.KeyChar == '=')
            {
                // Enter or '=' -> evaluate full expression (supports parentheses)
                var expr = txtinput1.Text;
                // if user hasn't typed expression, try using constructed expression
                if (string.IsNullOrEmpty(expr) && !string.IsNullOrEmpty(_expression))
                    expr = _expression;

                var (ok, val) = EvaluateExpression(expr);
                if (!ok)
                {
                    // if divide by zero or parse error, show message
                    txtresult1.Text = "0으로 나눌수 없습니다.";
                    txtresult1.ForeColor = System.Drawing.Color.Red;
                    _isError = true;
                }
                else
                {
                    txtresult1.Text = val.ToString();
                    txtresult1.ForeColor = val < 0 ? System.Drawing.Color.Red : System.Drawing.Color.Black;
                    txtinput1.Text = expr + "=" + val.ToString();
                    _expression = txtinput1.Text;
                }
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
