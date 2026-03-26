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

        // '=' 버튼: 두 피연산자의 정수 덧셈 수행(요구사항에 따라 Int 변환)
        private void btneq_Click(object sender, EventArgs e)
        {
            // If no operator set, do nothing (like Windows calc)
            if (string.IsNullOrEmpty(oprinput))
                return;

            // If current input is empty (operator was last input), show last operand as result
            if (string.IsNullOrEmpty(_currentInput))
            {
                result = operand1;
                var exprEmptyOperand = operand1.ToString() + oprinput + "=" + result.ToString();
                txtinput1.Text = exprEmptyOperand;
                txtresult1.Text = result.ToString();
                _currentInput = result.ToString();
                _expression = exprEmptyOperand;
                oprinput = string.Empty;
                operand1 = result;
                _isResultDisplayed = true;
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

            // Perform operation (only + implemented)
            if (oprinput == "+")
            {
                result = operand1 + operand2;
            }
            else
            {
                result = operand1 + operand2;
            }

            // Display without spaces
            var expr = operand1.ToString() + oprinput + operand2.ToString() + "=" + result.ToString();
            txtinput1.Text = expr;
            txtresult1.Text = result.ToString();

            _currentInput = result.ToString();
            _expression = expr;
            oprinput = string.Empty;
            operand1 = result;
            _isResultDisplayed = true;
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

            // Prevent duplicate operator: if operator already set and no current input, replace it
            if (!string.IsNullOrEmpty(oprinput) && string.IsNullOrEmpty(_currentInput))
            {
                // replace last operator in expression
                if (!string.IsNullOrEmpty(_expression))
                    _expression = _expression[..^1] + op;
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
            _expression += op;
            txtinput1.Text = _expression;
            // 현재 입력 초기화해서 두번째 피연산자 입력 준비
            _currentInput = string.Empty;
            txtresult1.Text = string.Empty;
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

            if (e.KeyChar == '\r' || e.KeyChar == '=')
            {
                // Enter or '=' -> execute
                btneq_Click(this, EventArgs.Empty);
                e.Handled = true;
                return;
            }

            // 차단: 문자 등 다른 입력은 무시
            e.Handled = true;
        }
    }
}
