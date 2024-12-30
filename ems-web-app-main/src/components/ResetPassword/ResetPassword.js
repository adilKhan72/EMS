import React, { Component } from "react";
import { Link } from "react-router-dom";
import logo from "../../images/rezaid-logo.png";
import queryString from "query-string";
import axios from "axios";
import PopUpModalBlack from "../PopUpModalBlack/PopUpModalBlack";
import Loader from "../Loader/Loader";
import "../Login/InputFieldColor.css";

class ResetPassword extends Component {
  constructor(props) {
    super(props);
    this.state = {
      showFileds: false,
      Title: "",
      Msg: "",
      PopUpBit: false,
      loginID: null,
      NewPassword: "",
      ConfrimNewPassword: "",
      Loading: false,
      passwordToggleNew: "password",
      passwordToggleConfrim: "password",
      eyeIconNew: "fa fa-eye-slash",
      eyeIconConfrim: "fa fa-eye-slash",
    };
    this.handleResetPassword = this.handleResetPassword.bind(this);
    this.ResetPassword = this.ResetPassword.bind(this);
  }
  CloseModal = () => {
    this.setState({ PopUpBit: false });
    if (this.state.Title == "Error") {
      this.props.history.push("/");
    } else if (this.state.Title == "Success") {
      this.props.history.push("/");
    }
  };
  componentDidMount() {
    try {
      const value = queryString.parse(this.props.location.search);
      if (value.WarningMsg == 1) {
        this.setState({ loginID: value.LI });
        this.setState({ showFileds: true });
      } else {
        this.setState({ Loading: true });
        this.setState({ loginID: value.LI });
        const globalIDObj = {
          globalID: value.GI,
        };

        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}ResetPassword/ResetPasswordValidation`,
          headers: {},
          data: globalIDObj,
        }).then((response) => {
          this.setState({ Loading: false });
          if (response.data.Result == 1) {
            this.setState({ showFileds: true });
          } else {
            this.setState({ Title: "Error" });
            this.setState({ Msg: response.data.Msg });
            this.setState({ PopUpBit: true });
          }
        });
      }
    } catch (ex) {
      window.location.href = "/Error";
    }
  }

  ResetPassword(e) {
    if (this.state.NewPassword === "" && this.state.ConfrimNewPassword === "") {
      this.setState({ Title: "Empty Field Error" });
      this.setState({ Msg: "Fields cannot be Empty" });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.NewPassword == "") {
      this.setState({ Title: "Empty Field Error" });
      this.setState({ Msg: "New Password cannot be Empty" });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.ConfrimNewPassword == "") {
      this.setState({ Title: "Empty Field Error" });
      this.setState({ Msg: "Confrim New Password cannot be Empty" });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.NewPassword !== this.state.ConfrimNewPassword) {
      this.setState({ Title: "Field Error" });
      this.setState({
        Msg: "New Password and Confrim New Password must be same!",
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else {
      try {
        this.setState({ Loading: true });
        const ResetPasswordObj = {
          LoginID: this.state.loginID,
          changePassword: this.state.NewPassword,
        };

        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}ResetPassword/ResetPassword`,
          headers: {},
          data: ResetPasswordObj,
        }).then((res) => {
          this.setState({ Loading: false });
          if (res.data.isSuccess == true) {
            if (res.data.Result.ResponseCode == 1) {
              this.setState({ Title: "Success" });
              this.setState({ Msg: res.data.Result.ResponseMessage });
              this.setState({ PopUpBit: true });
              e.preventDefault();
            } else {
              this.setState({ Title: "Failed" });
              this.setState({ Msg: res.data.Result.ResponseMessage });
              this.setState({ PopUpBit: true });
              e.preventDefault();
            }
          } else {
            this.setState({ Title: "Error" });
            this.setState({ Msg: res.data.Result.ResponseMessage });
            this.setState({ PopUpBit: true });
            e.preventDefault();
          }
        });
      } catch (ex) {
        window.location.href = "/Error";
      }
    }
    e.preventDefault();
  }

  handleResetPassword(e) {
    try {
      var target = e.target;
      var value = target.value;
      var name = target.name;
      this.setState({
        [name]: value,
      });
    } catch (ex) {
      alert(ex);
    }
  }
  TogglePasswordNew = (e) => {
    if (this.state.passwordToggleNew == "text") {
      this.setState({ eyeIconNew: "fa fa-eye-slash" });
      this.setState({ passwordToggleNew: "password" });
    } else {
      this.setState({ passwordToggleNew: "text" });
      this.setState({ eyeIconNew: "fa fa-eye" });
    }
  };
  TogglePasswordConfrim = (e) => {
    if (this.state.passwordToggleConfrim == "text") {
      this.setState({ eyeIconConfrim: "fa fa-eye-slash" });
      this.setState({ passwordToggleConfrim: "password" });
    } else {
      this.setState({ passwordToggleConfrim: "text" });
      this.setState({ eyeIconConfrim: "fa fa-eye" });
    }
  };
  render() {
    const eyestyle = {
      color: "white",
      cursor: "pointer",
      float: "right",
      marginLeft: "-25px",
      marginTop: "-30px",
      marginRight: "10px",
      position: "relative",
      zIndex: 2,
    };
    return (
      <div className="bg-theme-black" style={{ height: "100vh" }}>
        <div className="sufee-login d-flex align-content-center flex-wrap">
          <div className="container">
            <div className="login-content">
              <div className="login-logo">
                <a>
                  <Link to="/">
                    <img src={logo} alt="Logo" />
                  </Link>
                </a>

                <h2
                  className="text-center"
                  style={{ color: "white", paddingTop: "20px" }}
                >
                  Reset your password
                </h2>
                {this.state.showFileds == true ? (
                  <div class="login-form">
                    <form>
                      <div class="form-group InputBlack">
                        <input
                          type={this.state.passwordToggleNew}
                          style={{ backgroundColor: "black", color: "white" }}
                          onChange={this.handleResetPassword}
                          name="NewPassword"
                          value={this.state.NewPassword}
                          class="form-control"
                          placeholder="New Password"
                        />
                        <i
                          className={this.state.eyeIconNew}
                          onClick={this.TogglePasswordNew}
                          style={eyestyle}
                        ></i>
                      </div>
                      <div class="form-group InputBlack">
                        <input
                          type={this.state.passwordToggleConfrim}
                          style={{ backgroundColor: "black", color: "white" }}
                          onChange={this.handleResetPassword}
                          name="ConfrimNewPassword"
                          value={this.state.ConfrimNewPassword}
                          class="form-control"
                          placeholder="Confirm New Password"
                        />
                        <i
                          className={this.state.eyeIconConfrim}
                          onClick={this.TogglePasswordConfrim}
                          style={eyestyle}
                        ></i>
                      </div>
                    </form>
                    <div className="d-flex flex-row">
                      <button
                        type="submit"
                        className="btn-theme m-b-30 m-t-30"
                        onClick={this.ResetPassword}
                      >
                        Submit
                      </button>
                    </div>
                  </div>
                ) : (
                  <div></div>
                )}
                <div class="copyright" style={{ color: "white" }}>
                  &copy; 2021 Rezaid All rights reserved.
                </div>
              </div>
            </div>
          </div>
        </div>
        <PopUpModalBlack
          Title={this.state.Title}
          errorMsgs={this.state.Msg}
          show={this.state.PopUpBit}
          onHide={this.CloseModal}
        />
        {this.state.Loading ? <Loader /> : null};
      </div>
    );
  }
}

export default ResetPassword;
