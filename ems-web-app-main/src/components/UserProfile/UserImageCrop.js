import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import "../PopUpMsgModal/stylepopup.css";
import axios from "axios";
import PopUpModal from "../PopUpMsgModal/PopUpModal";
import Cookies from "js-cookie";
import profilelogos from "../../images/UserProfileAvatar.png";
import getCroppedImg from "./cropImage";
import "./CropStyle.css";
import Slider from "@material-ui/core/Slider";
import Cropper from "react-easy-crop";
import { FileDrop } from "react-file-drop";

class UserImageCrop extends React.Component {
  state = {
    imageSrc: null,
    crop: { x: 0, y: 0 },
    zoom: 1,
    //cropImg: null,
    croppedAreaPixels: null,
    aspect: 1,
    errorMsg: "",
    Title: "",
    PopUpBitImg: false,
    preview: profilelogos,
    imgSrc: null,
    base64Image: null,
  };
  onCropChange = (crop) => {
    this.setState({ crop });
  };

  onCropComplete = (croppedArea, croppedAreaPixels) => {
    console.log(croppedAreaPixels.width / croppedAreaPixels.height);
    this.setState({ croppedAreaPixels: croppedAreaPixels }, async () => {
      const croppedImage = await getCroppedImg(
        this.state.imageSrc,
        this.state.croppedAreaPixels
      );
      this.setState({
        imgSrc: croppedImage,
      });
      this.setState({ preview: croppedImage });
    });
  };

  onZoomChange = (zoom) => {
    this.setState({ zoom });
  };
  CloseModal = () => {
    this.setState({ PopUpBitImg: false });
    this.setState({ preview: profilelogos });
    this.setState({ crop: { x: 0, y: 0 } });
    this.setState({ zoom: 1 });
    this.setState({ croppedAreaPixels: null });
    this.setState({ aspect: 1 });
    this.setState({ imageSrc: null });
    this.setState({ imgSrc: null });
    this.setState({ base64Image: null });

    this.props.onHide();
  };

  ImgSeletedHandler = (e) => {
    try {
      if (e.target.files[0].size > 1000000) {
        this.setState({ Title: "Error" });
        this.setState({
          errorMsg: "Pic size is too big. It should be less then 1mb!!!",
        });
        this.setState({ PopUpBitImg: true });
        e.target.value = "";
      } else {
        this.setState({ picture: e.target.files[0] });
        var file = e.target.files[0];
        var reader = new FileReader();
        var url = reader.readAsDataURL(file);
        reader.onloadend = function (e) {
          this.setState({ imageSrc: [reader.result] });
        }.bind(this);
      }
    } catch (e) {
      alert(e);
    }
  };
  imgUpoad = () => {
    try {
      if (this.state.imageSrc != null) {
        var base64string = this.state.imgSrc;
        var xhr = new XMLHttpRequest(); //convert that blob to base64
        xhr.responseType = "blob";
        xhr.onload = async function () {
          var recoveredBlob = xhr.response;
          var reader = new FileReader();
          reader.onload = async function () {
            var blobAsDataUrl = reader.result;
            await this.setState({ base64Image: blobAsDataUrl }, () => {
              var base64 = this.state.base64Image;
              var index = base64.indexOf(",");
              var UserImgFile = base64.substr(index + 1);

              const Imagefileobj = {
                UserId: Cookies.get("UserID"),
                Imagefileobj: UserImgFile,
              };
              axios({
                method: "post",
                url: `${process.env.REACT_APP_BASE_URL}User/UploadFiles`,
                headers: {
                  Authorization:
                    "Bearer " + localStorage.getItem("access_token"),
                  encrypted: localStorage.getItem("EncryptedType"),
                  Role_Type: Cookies.get("Role"),
                },
                data: Imagefileobj,
              }).then((res) => {
                if (res.data?.StatusCode === 401) {
                  this.logout();
                }
                this.setState({ Title: "Success" });
                this.setState({ errorMsg: res.data.Result });
                this.setState({ PopUpBitImg: true });
                console.log(res);
              });
            });
          }.bind(this);
          var test = reader.readAsDataURL(recoveredBlob);
        }.bind(this);
        xhr.open("GET", base64string);
        xhr.send();
      } else {
        this.setState({ Title: "Error" });
        this.setState({
          errorMsg: "Please Select an Image",
        });
        this.setState({ PopUpBitImg: true });
      }
    } catch (e) {
      console.log(e);
    }
  };
  render() {
    return (
      <Modal
        show={this.props.show}
        onHide={this.CloseModal}
        backdrop={"static"}
        keyboard={"false"}
        centered
        dialogClassName="CustomWidthPic upload_img_modal "
      >
        <Modal.Header closeButton style={{ border: "0px" }}>
          <Modal.Title>{this.props.Title}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className=" profile_popup">
            <div
              className="col-sm-12 popup_grid"
              style={{
                display: "flex",
                paddingTop: "20px",
              }}
            >
              <div className="col-4 text-center">
                <div>
                  <FileDrop
                    onDrop={(files, event) => {
                      if (files[0].size > 1000000) {
                        this.setState({ Title: "Error" });
                        this.setState({
                          errorMsg:
                            "Pic size is too big. It should be less then 1mb!!!",
                        });
                        this.setState({ PopUpBitImg: true });
                      } else {
                        this.setState({ picture: files[0] });
                        var file = files[0];
                        var reader = new FileReader();
                        var url = reader.readAsDataURL(file);
                        reader.onloadend = function (e) {
                          this.setState({ imageSrc: [reader.result] });
                        }.bind(this);
                      }
                      console.log("onDrop!", files, event);
                    }}
                  >
                    <img src={this.state.preview} alt="Preview" />

                    <div
                      class="add-avator"
                      for="file-input"
                      style={{
                        position: "relative",
                        left: "80px",
                        top: "-35px",
                      }}
                    >
                      <form enctype="multipart/form-data">
                        <label for="file-input" style={{ width: "30px" }}>
                          <input
                            className="file-upload"
                            id="file-input"
                            type="file"
                            accept="image/*"
                            onChange={this.ImgSeletedHandler}
                          />
                          <i class="fa fa-plus upload-button"></i>
                        </label>
                      </form>
                    </div>
                    {this.state.imageSrc == null ? (
                      <label className="label_img_text">
                        <span>Drop file here or browse</span>
                      </label>
                    ) : null}
                  </FileDrop>
                </div>
                <div>130 x 130</div>
              </div>

              <div className="col-8">
                <div
                  style={{
                    position: "relative",
                    backgroundColor: "lightgrey",
                    borderRadius: "15px",
                    height: "225px",
                    maxWidth: "400px",
                  }}
                >
                  <label className="user_pw_label">
                    <span
                      style={{
                        position: "absolute",
                        top: "38px",
                        left: "17px",
                        fontWeight: "bold",
                      }}
                    >
                      Preview
                    </span>
                  </label>
                  <div
                    style={{
                      cursor: "pointer",
                      borderRadius: "15px",
                      backgroundColor: "#B8B8B8",
                    }}
                    className="crop-container"
                  >
                    <Cropper
                      image={this.state.imageSrc}
                      crop={this.state.crop}
                      zoom={this.state.zoom}
                      aspect={this.state.aspect}
                      cropShape="round"
                      showGrid={false}
                      onCropChange={this.onCropChange}
                      onCropComplete={this.onCropComplete}
                      onZoomChange={this.onZoomChange}
                    />
                  </div>
                  <div
                    style={{
                      position: "relative",
                      top: "120px",
                      display: "flex",
                    }}
                    className="cropstyle"
                  >
                    <i
                      class="fa fa-picture-o"
                      style={{
                        fontSize: "20px",
                        padding: "5px",
                        marginRight: "8px",
                      }}
                      aria-hidden="true"
                    ></i>

                    <Slider
                      value={this.state.zoom}
                      min={1}
                      max={3}
                      step={0.1}
                      aria-labelledby="Zoom"
                      onChange={(e, zoom) => this.onZoomChange(zoom)}
                      classes={{ container: "slider" }}
                      style={{ color: "#575550" }}
                    />

                    <i
                      class="fa fa-picture-o"
                      style={{
                        fontSize: "20px",
                        padding: "5px",
                        marginLeft: "8px",
                      }}
                      aria-hidden="true"
                    ></i>
                  </div>
                </div>
              </div>
            </div>
            <Button
              className="btn-black"
              onClick={this.imgUpoad}
              style={{
                marginLeft: "7.2%",
                marginBottom: "10px",
              }}
            >
              Upload
            </Button>
          </div>
        </Modal.Body>
        <Modal.Footer style={{ border: "0px", paddingTop: "0px" }}>
          <div className="w-100 text-left "></div>
        </Modal.Footer>

        <PopUpModal
          Title={this.state.Title}
          errorMsgs={this.state.errorMsg}
          show={this.state.PopUpBitImg}
          onHide={this.CloseModal}
        />
      </Modal>
    );
  }
}

export default UserImageCrop;
