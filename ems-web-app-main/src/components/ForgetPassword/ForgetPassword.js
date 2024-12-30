import React, { Component } from "react";
import logo from "../../images/rezaid-logo.png";
import PopUpModalBlack from "../PopUpModalBlack/PopUpModalBlack";
import axios from "axios";
import { Link } from "react-router-dom";
import Loader from "../Loader/Loader";
import "../Login/InputFieldColor.css";

class ForgetPassword extends Component {
  constructor(props) {
    super(props);
    this.state = {
      email: "",
      Title: "",
      Msg: "",
      PopUpBit: false,
      Loading: false,
    };
  }

  CloseModal = () => {
    this.setState({ PopUpBit: false });
    if (this.state.Title == "Success") {
      this.props.history.push("/");
    }
  };
  ForgetPass = (e) => {
    if (this.state.email == "") {
      this.setState({ Title: "Error" });
      this.setState({ Msg: "Email field cannot be Empty" });
      this.setState({ PopUpBit: true });
    } else {
      try {
        this.setState({ Loading: true });
        const userObj = {
          EmailAddress: this.state.email,
        };
        if (!/^[a-zA-Z0-9]+@[a-zA-Z0-9]+\.[A-Za-z]+$/.test(this.state.email)) {
          this.setState({
            Title: "Email Address Invalid",
          });
          this.setState({
            Msg: `Please Enter Valid Email Address`,
          });
          this.setState({ PopUpBit: true, Loading: false });
          return false;
        }
        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}ForgetPassword/forgetPassword`,
          headers: {},
          data: userObj,
        }).then((response) => {
          this.setState({ Loading: false });
          if (response.data.isSuccess == true) {
            this.setState({ Title: "Success" });
            this.setState({
              Msg: "A reset password link has been sent to your email address",
            });
            this.setState({ PopUpBit: true });
          } else {
            this.setState({ Title: "Error" });
            this.setState({ Msg: response.data.ResponseMsg });
            this.setState({ PopUpBit: true });
            console.log("errrr");
            e.preventDefault();
          }
        });
      } catch (ex) {
        window.location.href = "/Error";
      }
    }
    e.preventDefault();
  };

  handleforgetpass = (e) => {
    var target = e.target;
    var value = target.value;
    var name = target.name;
    this.setState({
      [name]: value,
    });
  };

  render() {
    return (
      <div>
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
                </div>
                <h2 className="text-center" style={{ color: "white" }}>
                  Forget Password
                </h2>
                <div className="login-form material_style_login">
                  <form>
                    <div className="form-group InputBlack">
                      {this.state.Loading ? <Loader /> : null};
                      <input
                        type="email"
                        style={{ backgroundColor: "black", color: "white" }}
                        onChange={this.handleforgetpass}
                        name="email"
                        value={this.state.email}
                        className="form-control"
                        //placeholder="Your Email Address"
                        required
                      />
                      <label>Your Email Address</label>
                    </div>
                    <button
                      type="submit"
                      className="btn-theme m-b-30 m-t-30"
                      onClick={this.ForgetPass}
                    >
                      Submit
                    </button>
                  </form>
                </div>
                <div
                  className="copyright text-center"
                  style={{ color: "white" }}
                >
                  &copy; {new Date().getFullYear()} Rezaid All rights reserved.
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
      </div>
    );
  }
}

export default ForgetPassword;
