import React, { Component } from "react";
import logo from "../../images/rezaid-logo.png";
import PopUpModalBlack from "../PopUpModalBlack/PopUpModalBlack";
import axios from "axios";
import Loader from "../Loader/Loader";
import moment from "moment";
import Cookies from "js-cookie";
import "./InputFieldColor.css";
import { connect } from "react-redux";
import { isLoginAction } from "../Redux/Actions/index";
import {
  BrowserRouter as Router,
  // Switch,
  // Route,
  Link,
  useRouteMatch,
} from "react-router-dom";
import { Alert } from "react-bootstrap";
import { encrypt, decrypt } from "react-crypt-gsm";

class Login extends Component {
  constructor(props) {
    super(props);
    this.state = {
      username: "",
      password: "",
      errorMsg: "",
      Title: "Login Error",
      PopUpBit: false,
      Loading: false,
      passwordToggle: "password",
      eyeIcon: "fa fa-eye-slash",
    };

    this.handleLogin = this.handleLogin.bind(this);
    this.LoginUser = this.LoginUser.bind(this);
  }

  componentDidMount() {
    Cookies.remove("Role");
    Cookies.remove("UserID");
    Cookies.remove("UserName");
    localStorage.setItem("access_token", "");
    localStorage.setItem("login_time", "");
    localStorage.setItem("expires_in", "");
  }
  LoginUser(e) {
    if (this.state.username === "" && this.state.password === "") {
      this.setState({ errorMsg: "Username and Password cannot be Empty" });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.username === "") {
      this.setState({ errorMsg: "Username cannot be Empty" });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.password === "") {
      this.setState({ errorMsg: "Password cannot be Empty" });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else {
      try {
        this.setState({ Loading: true });
        var bodyData =
          "username=" +
          this.state.username +
          "&password=" +
          this.state.password +
          "&grant_type=password";

        const userObj = {
          username: this.state.username,
          password: this.state.password,
        };
        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}Login/LoginAPI`,
          headers: {},
          data: userObj,
        }).then((res) => {
          if (res.data.isSuccess === true) {
            axios({
              method: "post",
              url: `${process.env.REACT_APP_BASE_URL}token`,
              headers: {
                "Content-type":
                  "application/x-www-form-urlencoded; charset=utf-8",
              },
              data: bodyData,
            })
              .then((data) => {
                const encryptedStr = encrypt(res.data.Result.Role);
                localStorage.setItem(
                  "EncryptedType",
                  res?.data?.EncryptedRoleType
                );
                localStorage.setItem("access_token", data.data.access_token);
                localStorage.setItem("expires_in", data.data.expires_in);
                Cookies.set("encrypted_Type", encryptedStr.content, {
                  expires: 2,
                });
                Cookies.set("encrypted_Type_Length", encryptedStr.tag, {
                  expires: 2,
                });
                // localStorage.setItem("encrypted_Type", encryptedStr.content);
                // localStorage.setItem("encrypted_Type_Length", encryptedStr.tag);
                localStorage.setItem(
                  "login_time",
                  moment(new Date()).format("MM-DD-YYYY HH:mm:ss")
                );
                this.setState({ Loading: false });
              })
              .finally(() => {
                if (
                  res.data.Result.Role === "SuperAdmin" ||
                  res.data.Result.Role === "Admin"
                ) {
                  Cookies.set("UserName", res.data.Result.FullName, {
                    expires: 2,
                  });
                  Cookies.set("UserID", res.data.Result.UserId, { expires: 2 });
                  Cookies.set("Role", res.data.Result.Role, { expires: 2 });
                  Cookies.set("Designation", res.data.Result.Designation, {
                    expires: 2,
                  });

                  this.props.history.push("/AdminDashboard");
                } else {
                  Cookies.set("UserName", res.data.Result.FullName, {
                    expires: 2,
                  });
                  Cookies.set("UserID", res.data.Result.UserId, { expires: 2 });
                  Cookies.set("Role", res.data.Result.Role, { expires: 2 });
                  Cookies.set("Designation", res.data.Result.Designation, {
                    expires: 2,
                  });
                  this.props.history.push("/UserDashboard");
                }
              });
          } else {
            this.setState({ Loading: false });
            this.setState({ errorMsg: res.data.ResponseMsg });
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

  handleLogin(e) {
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
  TogglePassword = (e) => {
    if (this.state.passwordToggle === "text") {
      this.setState({ eyeIcon: "fa fa-eye-slash" });
      this.setState({ passwordToggle: "password" });
    } else {
      this.setState({ passwordToggle: "text" });
      this.setState({ eyeIcon: "fa fa-eye" });
    }
  };
  CloseModal = () => {
    this.setState({ PopUpBit: false });
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
      <div className="bg-theme-black " style={{ height: "100vh" }}>
        <div className="sufee-login d-flex align-content-center flex-wrap">
          <div className="container">
            <div className="login-content">
              <div className="login-logo">
                <a href="index.html">
                  <img src={logo} alt="Logo" />
                </a>
              </div>
              <h2 className="text-center" style={{ color: "white" }}>
                Effort Management System
              </h2>
              <div className="login-form material_style_login">
                <form>
                  <div className="form-group InputBlack">
                    <input
                      type="text"
                      style={{ backgroundColor: "black", color: "white" }}
                      onChange={this.handleLogin}
                      name="username"
                      // value={this.state.username}
                      className="form-control InputFieldColor"
                      placeholder=" "
                    />
                    <label>User Name</label>
                  </div>
                  <div className="form-group InputBlack">
                    <input
                      type={this.state.passwordToggle}
                      style={{ backgroundColor: "black", color: "white" }}
                      onChange={this.handleLogin}
                      name="password"
                      value={this.state.password}
                      className="form-control InputFieldColor"
                      placeholder=" "
                    />
                    <i
                      className={this.state.eyeIcon}
                      onClick={this.TogglePassword}
                      style={eyestyle}
                    ></i>
                    <label>Password</label>
                  </div>
                  <div className="form-group mb-o">
                    <button
                      type="submit"
                      onClick={this.LoginUser}
                      className="btn-theme m-b-30 m-t-30"
                    >
                      Login
                    </button>
                  </div>
                  {/* <div className="checkbox mt-4 mb-4">
                          <label>
                            <input type="checkbox" /> Remember Me
                          </label> 
                        </div> */}
                  <label className="pull-right">
                    <Link to="/ForgetPassword">Forgotten Password?</Link>
                  </label>
                  {this.state.Loading ? <Loader /> : null}
                </form>
              </div>
            </div>
            <div className="copyright text-center" style={{ color: "white" }}>
              &copy; {new Date().getFullYear()} Rezaid All rights reserved.
            </div>
          </div>
        </div>
        <PopUpModalBlack
          Title={this.state.Title}
          errorMsgs={this.state.errorMsg}
          show={this.state.PopUpBit}
          onHide={this.CloseModal}
        />
      </div>
    );
  }
}
/* const mapStateToProps = (state) => {
  return {
    isLogin: state.UserData,
  };
};
const mapDispatchToProps = (dispatch) => {
  return {
    isLoginAction: () => dispatch(isLoginAction()),
  };
}; */
export default Login /* connect(mapStateToProps, mapDispatchToProps)(Login) */;
