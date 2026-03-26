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
            // 두 번째 피연산자 가져오기
            if (!int.TryParse(_currentInput, out operand2))
                operand2 = 0;

            // operand1는 SetOperator에서 이미 저장되어 있어야 함
            if (oprinput == "+")
            {
                result = operand1 + operand2;
            }
            else
            {
                // 다른 연산자 미구현 시 기본 더하기
                result = operand1 + operand2;
            }

            // 수식 표기와 결과 출력
            _expression += _currentInput;
            _expression += "=" + result.ToString();
            txtinput1.Text = _expression;
            txtresult1.Text = result.ToString();

            // 연산 완료 후 상태 초기화(결과를 다음 연산의 첫 피연산자로 사용하고 싶다면 _currentInput=result.ToString())
            _currentInput = string.Empty;
            _expression = string.Empty;
            oprinput = string.Empty;
            operand1 = 0;
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
        }

        private void btnDEL_Click(object sender, EventArgs e)
        {
            // 현재 입력에서 마지막 문자 제거
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
            if (int.TryParse(_currentInput, out var v))
            {
                v = -v;
                _currentInput = v.ToString();
                // update expression: remove previous currentInput portion and append new
                // For simplicity, rebuild expression
                _expression = operand1 != 0 || !string.IsNullOrEmpty(oprinput) ? operand1.ToString() + oprinput + _currentInput : _currentInput;
                txtinput1.Text = _expression;
                txtresult1.Text = _currentInput;
            }
        }

        // Helper: append digit to current input and expression, update textboxes
        private void AppendDigit(string d)
        {
            _currentInput += d;
            _expression += d;
            txtinput1.Text = _expression;
            txtresult1.Text = _currentInput;
        }

        // Helper: set operator; store operand1 and append operator to expression
        private void SetOperator(string op)
        {
            // 저장된 현재 입력값을 첫 번째 피연산자로 변환
            if (!int.TryParse(_currentInput, out operand1))
                operand1 = 0;

            oprinput = op;
            _expression += op;
            txtinput1.Text = _expression;
            // 현재 입력 초기화해서 두번째 피연산자 입력 준비
            _currentInput = string.Empty;
            txtresult1.Text = string.Empty;
        }
    }
}
