import React, { Component } from "react";
import axios from "axios";
import Loader from "../Loader/Loader";
import PopUpModal from "../PopUpMsgModal/PopUpModal";
import UserImageCrop from "./UserImageCrop";
import Cookies from "js-cookie";
import NoPic from "../../images/profile-avator.png";
import { RefreshAction } from "../Redux/Actions/index";
import { connect } from "react-redux";
import "./userprofile.css";
import { encrypt, decrypt } from "react-crypt-gsm";

class UserProfile extends Component {
  constructor(props) {
    super(props);
    this.state = {
      lstUserProfile: null,
      Name: "",
      Email: "",
      Department: "",
      Designation: "",
      EMSRole: "",
      Status: "",
      JoiningDate: "",
      picture: "",
      Loading: true,
      errorMsg: "",
      Title: "",
      PopUpBit: false,
      ShowPicUploadModal: false,
    };

    this.onFocus = this.onFocus.bind(this);
    this.onBlur = this.onBlur.bind(this);
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
      localStorage.removeItem("token");
      localStorage.removeItem("Video_Upload_Size");
      localStorage.removeItem("S3Obj");
      localStorage.removeItem("userId");
      localStorage.removeItem("EncryptedType");
      localStorage.removeItem("access_token");
      localStorage.removeItem("login_time");
      localStorage.removeItem("expires_in");
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
  onFocus(e) {
    e.currentTarget.type = "date";
    e.currentTarget.focus();
  }
  onBlur(e) {
    e.currentTarget.type = "text";
    e.currentTarget.blur();
  }

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
        this.setState({ lstUserProfile: res.data.Result });
        var temName;
        var tempEmail;
        var tempDepartment;
        var tempDesignation;
        var tempJoiningDate;
        var tempStatus;
        var tempEMSRole;
        var tempImg;
        if (this.state.lstUserProfile !== null) {
          this.state.lstUserProfile.map((item) => {
            temName = item.Name;
            tempEmail = item.Email;
            tempDepartment = item.Department;
            tempDesignation = item.Designation;
            tempJoiningDate = item.JoiningDate;
            tempStatus = item.Status;
            tempEMSRole = item.EMSRole;
            tempImg = item.Picture;
          });
        }
        this.setState({ Name: temName });
        this.setState({ Email: tempEmail });
        this.setState({ Department: tempDepartment });
        this.setState({ Designation: tempDesignation });
        this.setState({ JoiningDate: tempJoiningDate });
        this.setState({ Status: tempStatus });
        this.setState({ EMSRole: tempEMSRole });
        this.setState({ picture: tempImg });

        this.setState({ Loading: false });
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  CloseModal = () => {
    this.props.RefreshAction(); //Set the redux state to true
    this.setState({ PopUpBit: false });
    this.setState({ ShowPicUploadModal: false });
  };
  render() {
    return (
      <div>
        <div className="content mt-3">
          <div className="row">
            <div className="col-sm-12">
              <div className="row align-items-center">
                <div className="col-sm-6" style={{ marginBottom: "25px" }}>
                  <h3 className="m_font_heading user_left_padding">
                    User Profile
                  </h3>
                </div>
                {/* <div className="col-sm-6 text-right">
                               <button type="button" className="btn-black mr-2">Edit User <i className="fa fa-pencil"></i></button>
                            </div> */}
              </div>
              <div className="card">
                <div className="card-body material_style">
                  {this.state.Loading ? (
                    <Loader />
                  ) : (
                    <form className="col-md-12" noValidate autoComplete="off">
                      <div
                        className="row align-items-center"
                        style={{ marginBottom: "-18px" }}
                      >
                        <div class="row align-items-center">
                          <div class="col-md-2">
                            <div class="form-group">
                              <div class="profile-placeholder">
                                <div class="circle">
                                  <img
                                    class="profile-pic"
                                    src={`http://localhost:1316userimages/UserImgID${Cookies.get(
                                      "UserID"
                                    )}.png?cache=${new Date()}`}
                                    alt="Userimage"
                                    onError={(e) => {
                                      e.target.onerror = null;
                                      e.target.src = NoPic;
                                    }}
                                  />
                                </div>
                                <div class="add-avator" for="file-input">
                                  <form enctype="multipart/form-data">
                                    <label>
                                      <i
                                        class="fa fa-plus upload-button"
                                        onClick={() => {
                                          this.setState({
                                            ShowPicUploadModal: true,
                                          });
                                        }}
                                      ></i>
                                    </label>
                                  </form>
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>

                      <div
                        className="row ems_read_only_data"
                        style={{ marginTop: "20px" }}
                      >
                        <div className="col-md-4">
                          <div className="form-group mb-1">
                            <input
                              type="text"
                              // placeholder="UserName"
                              class="form-control"
                              defaultValue={this.state.Name}
                              required={true}
                            />
                            <label>User Name</label>
                          </div>
                        </div>
                        <div className="col-md-4">
                          <div className="form-group mb-1">
                            <input
                              type="text"
                              // placeholder="Email"
                              class="form-control"
                              defaultValue={this.state.Email}
                              required
                            />
                            <label>Email</label>
                          </div>
                        </div>
                        <div className="col-md-4">
                          <div className="form-group mb-1">
                            <input
                              type="text"
                              //placeholder="JoiningDate"
                              class="form-control"
                              defaultValue={this.state.JoiningDate}
                              required
                            />
                            <label>Joining Date</label>
                          </div>
                        </div>
                        <div className="col-md-4">
                          <div className="form-group mb-1">
                            <input
                              type="text"
                              // placeholder="Department"
                              class="form-control"
                              defaultValue={this.state.Department}
                              required
                            />
                            <label>Department</label>
                          </div>
                        </div>

                        <div className="col-md-4">
                          <div className="form-group mb-1">
                            <input
                              type="text"
                              // placeholder="Designation"
                              class="form-control"
                              defaultValue={this.state.Designation}
                              required
                            />
                            <label>Designation</label>
                          </div>
                        </div>

                        <div className="col-md-4">
                          <div className="form-group mb-1">
                            <input
                              type="text"
                              // placeholder="Status"
                              class="form-control"
                              defaultValue={
                                this.state.Status ? "Active" : "NotActive"
                              }
                              required
                            />
                            <label>Status</label>
                          </div>
                        </div>

                        <div className="col-md-4">
                          <div className="form-group mb-1">
                            <input
                              type="text"
                              //placeholder="EMSRole"
                              class="form-control"
                              defaultValue={this.state.EMSRole}
                              required
                            />
                            <label>EMS Role</label>
                          </div>
                        </div>
                      </div>

                      {/* <div className="col-md-12">
                        <input type="submit" value="Save Changes"/>
                      </div> */}
                    </form>
                  )}
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
          <UserImageCrop
            Title={"Upload Photo"}
            show={this.state.ShowPicUploadModal}
            onHide={this.CloseModal}
            Upload={this.imgUploadHnadler}
          />
        </div>
      </div>
    );
  }
}
const mapStateToProps = (state) => {
  return {
    Refresh: state.RefreshBit,
  };
};
const mapDispatchToProps = (dispatch) => {
  return {
    RefreshAction: () => dispatch(RefreshAction()),
  };
};
export default connect(mapStateToProps, mapDispatchToProps)(UserProfile);
