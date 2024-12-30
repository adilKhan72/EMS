import React, { Component } from "react";
import axios from "axios";
import PopUpModal from "../PopUpMsgModal/PopUpModal";
import Loader from "../Loader/Loader";
import Cookies from "js-cookie";
import { encrypt, decrypt } from "react-crypt-gsm";
const errorstyle = {
  borderColor: "#f86c6b",
};
class ChangePassword extends Component {
  constructor(props) {
    super(props);
    this.state = {
      GetOldPassword: "",
      oldpassword: "",
      newpassword: "",
      confirmPassword: "",
      errorMsg: "",
      Title: "Change Password Error",
      PopUpBit: false,
      success: false,
      Loading: false,
      passwordToggleOld: "password",
      passwordToggleNew: "password",
      passwordToggleConfrim: "password",
      eyeIconOld: "fa fa-eye-slash",
      eyeIconNew: "fa fa-eye-slash",
      eyeIconConfrim: "fa fa-eye-slash",
      errorStyle: {
        OldPassword: null,
        NewPassword: null,
        ConfrimPassword: null,
      },
    };
    this.handlePassword = this.handlePassword.bind(this);
    this.ConfrimPassword = this.ConfrimPassword.bind(this);
  }
  componentDidMount() {
    const GetKey = Cookies.get("encrypted_Type");
    const Getbytes = JSON.parse(Cookies.get("encrypted_Type_Length"));
    const GetEncryptedFormat = { content: GetKey, tag: Getbytes.data };
    const decryptValue = decrypt(GetEncryptedFormat);
    const CookieValue = Cookies.get("Role");
    if (
      decryptValue != CookieValue &&
      (decryptValue != "Staff" ||
        decryptValue != "Admin" ||
        decryptValue != "SuperAdmin")
    ) {
      this.logout();
    }
    this.LoadUserProfile();
  }
  logout = () => {
    try {
      localStorage.setItem("access_token", "");
      localStorage.setItem("login_time", "");
      localStorage.setItem("expires_in", "");
      Cookies.remove("Role");
      Cookies.remove("UserID");
      Cookies.remove("UserName");
      Cookies.remove("Design");
      Cookies.remove("encrypted_Type");
      Cookies.remove("encrypted_Type_Length");
      window.location.href = ".";
    } catch (ee) {
      alert(ee);
    }
  };
  LoadUserProfile = () => {
    try {
      const UserIdObj = {
        UserID: Cookies.get("UserID"),
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}User/GetUserProfile`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: UserIdObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        if (res.data.Result.length > 0 && res.data.Result.length === 1) {
          this.setState({ GetOldPassword: res.data?.Result[0]?.Password });
        }
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ConfrimPassword(e) {
    if (
      this.state.oldpassword == "" &&
      this.state.newpassword == "" &&
      this.state.confirmPassword == ""
    ) {
      this.setState({ errorMsg: "Fields cannot be Empty" });
      this.setState({ PopUpBit: true });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          OldPassword: errorstyle,
          NewPassword: errorstyle,
          ConfrimPassword: errorstyle,
        },
      });
      e.preventDefault();
    } else if (this.state.oldpassword != this.state.GetOldPassword) {
      this.setState({
        errorMsg: "Old Password does not match.",
      });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          OldPassword: errorstyle,
          //NewPassword: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.oldpassword == this.state.newpassword) {
      this.setState({
        errorMsg: "Old Password cannot be same as New Password",
      });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          OldPassword: errorstyle,
          NewPassword: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.oldpassword == "") {
      this.setState({ errorMsg: "Old Password cannot be Empty" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          OldPassword: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.newpassword == "") {
      this.setState({ errorMsg: "New Password cannot be Empty" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          NewPassword: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.confirmPassword == "") {
      this.setState({ errorMsg: "Confirm Password cannot be Empty" });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ConfrimPassword: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    } else if (this.state.newpassword === this.state.confirmPassword) {
      try {
        this.setState({ Loading: true });
        const passObj = {
          UserId: Cookies.get("UserID"),
          oldpassword: this.state.oldpassword,
          newpassword: this.state.newpassword,
        };

        axios({
          method: "post",
          url: `${process.env.REACT_APP_BASE_URL}ChangePassword/UpdatePassword`,
          headers: {
            Authorization: "Bearer " + localStorage.getItem("access_token"),
            encrypted: localStorage.getItem("EncryptedType"),
            Role_Type: Cookies.get("Role"),
          },
          data: passObj,
        }).then((response) => {
          if (response.data?.StatusCode === 401) {
            this.logout();
          }
          this.setState({ Loading: false });

          if (
            response.data.StatusCode == 200 &&
            response.data.Result.ResponseCode == 1
          ) {
            this.setState({ Title: "Success" });
            this.setState({ errorMsg: "Password Change Successfully" });
            this.setState({ PopUpBit: true });
            this.setState({ success: true });
            e.preventDefault();
          } else {
            this.setState({ errorMsg: response.data.Result.ResponseMessage });
            this.setState({
              errorStyle: {
                ...this.state.errorStyle,
                OldPassword: errorstyle,
                NewPassword: errorstyle,
              },
            });
            this.setState({ PopUpBit: true });
            e.preventDefault();
          }
        });
      } catch (ex) {
        window.location.href = "/Error";
      }
    } else {
      this.setState({
        errorMsg: "New Password Does not match Confrim Password",
      });
      this.setState({
        errorStyle: {
          ...this.state.errorStyle,
          ConfrimPassword: errorstyle,
          NewPassword: errorstyle,
        },
      });
      this.setState({ PopUpBit: true });
      e.preventDefault();
    }
  }

  CloseModal = () => {
    if (this.state.success == true) {
      this.setState({ PopUpBit: false });
      Cookies.remove("Role");
      Cookies.remove("UserID");
      Cookies.remove("UserName");
      window.location.href = ".";
    } else {
      this.setState({ PopUpBit: false });
    }
  };

  handlePassword(e) {
    var target = e.target;
    var value = target.value;
    var name = target.name;
    this.setState({
      [name]: value,
    });
  }
  TogglePasswordOld = (e) => {
    if (this.state.passwordToggleOld == "text") {
      this.setState({ eyeIconOld: "fa fa-eye-slash" });
      this.setState({ passwordToggleOld: "password" });
    } else {
      this.setState({ passwordToggleOld: "text" });
      this.setState({ eyeIconOld: "fa fa-eye" });
    }
  };
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
  StyleRemoved = () => {
    this.setState({ errorStyle: {} });
  };
  render() {
    const eyestyle = {
      color: "black",
      cursor: "pointer",
      float: "right",
      marginLeft: "-25px",
      marginTop: "-30px",
      marginRight: "10px",
      position: "relative",
      zIndex: 2,
    };
    return (
      <div>
        <div className="card-header">
          <div className="ems_custom_padd_mr">
            <h3>Change Password</h3>
          </div>
        </div>
        <div className="content mt-4">
          <div className="row ems_updated_pass">
            <div className="col-md-12">
              <div className="card">
                <div className="card-body material_style">
                  <div className="form-group" style={{ marginTop: "10px" }}>
                    {/* <label className="col-md-12 ">
                          <h6>Old Password</h6>
                        </label> */}
                    <input
                      type={this.state.passwordToggleOld}
                      onChange={this.handlePassword}
                      name="oldpassword"
                      className="form-control"
                      style={this.state.errorStyle.OldPassword}
                      onFocus={this.StyleRemoved}
                      required
                    />
                    <i
                      className={this.state.eyeIconOld}
                      onClick={this.TogglePasswordOld}
                      style={eyestyle}
                    ></i>
                    <label>Old Password</label>
                  </div>
                  <div className="form-group">
                    {/*  <label className="col-md-12 ">
                          <h6>New Password</h6>
                        </label> */}
                    <input
                      type={this.state.passwordToggleNew}
                      onChange={this.handlePassword}
                      name="newpassword"
                      className="form-control"
                      style={this.state.errorStyle.NewPassword}
                      onFocus={this.StyleRemoved}
                      required
                    />
                    <i
                      className={this.state.eyeIconNew}
                      onClick={this.TogglePasswordNew}
                      style={eyestyle}
                    ></i>
                    <label>New Password</label>
                  </div>
                  <div className="form-group">
                    {/* <label className="col-md-12 ">
                          <h6>Confirm Password</h6>
                        </label> */}
                    <input
                      type={this.state.passwordToggleConfrim}
                      onChange={this.handlePassword}
                      name="confirmPassword"
                      className="form-control"
                      style={this.state.errorStyle.ConfrimPassword}
                      onFocus={this.StyleRemoved}
                      required
                    />
                    <i
                      className={this.state.eyeIconConfrim}
                      onClick={this.TogglePasswordConfrim}
                      style={eyestyle}
                    ></i>
                    <label>Confirm Password</label>
                  </div>
                  <div>
                    <button
                      type="submit"
                      onClick={this.ConfrimPassword}
                      className="btn-black btn_top_5 ems_change_pass_update"
                    >
                      Update
                    </button>
                    {/* <input
                          type="submit"
                          onClick={this.ConfrimPassword}
                          value="Save Changes"
                        /> */}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <PopUpModal
          Title={this.state.Title}
          errorMsgs={this.state.errorMsg}
          show={this.state.PopUpBit}
          onHide={this.CloseModal}
        />
        {this.state.Loading ? <Loader /> : ""}
      </div>
    );
  }
}

export default ChangePassword;
