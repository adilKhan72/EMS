import React, { Component } from "react";
import { Modal, Button, Form } from "react-bootstrap";
import "../PopUpMsgModal/stylepopup.css";
import Avatar from "react-avatar-edit";
import NoPic from "../../images/NoImage.PNG";
import profilelogo from "../../images/profile-avator.png";
import axios from "axios";
import PopUpModal from "../PopUpMsgModal/PopUpModal";
import Cookies from "js-cookie";
import PropTypes from "prop-types";
import { withStyles, makeStyles } from "@material-ui/core/styles";
import Slider from "@material-ui/core/Slider";
import Typography from "@material-ui/core/Typography";
import Tooltip from "@material-ui/core/Tooltip";
class UserImagePreview extends Component {
  constructor(props) {
    super(props);
    const src = "";
    this.state = {
      preview: profilelogo,
      src,
      Imagefile: null,
      errorMsg: "",
      Title: "",
      PopUpBitImg: false,
      silderValue: 40,
    };
    this.onCrop = this.onCrop.bind(this);
    this.onClose = this.onClose.bind(this);
    this.onBeforeFileLoad = this.onBeforeFileLoad.bind(this);
  }
  onClose() {
    this.setState({ preview: null });
  }

  onCrop(preview) {
    this.setState({ preview });
  }
  onBeforeFileLoad(elem) {
    try {
      if (elem.target.files[0].size > 1000000) {
        this.setState({ Title: "Error" });
        this.setState({
          errorMsg: "Pic size is too big. It should be less then 1mb!!!",
        });
        this.setState({ PopUpBitImg: true });
        elem.target.value = "";
      }
    } catch (e) {
      alert(e);
    }
  }
  imgUpoad = () => {
    try {
      var base64 = this.state.preview;
      var index = base64.indexOf(",");
      var UserImgFile = base64.substr(index + 1);
      this.setState({ Imagefile: UserImgFile });

      const Imagefileobj = {
        UserId: Cookies.get("UserID"),
        Imagefileobj: UserImgFile,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}User/UploadFiles`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
        },
        data: Imagefileobj,
      }).then((res) => {
        this.setState({ Title: "Success" });
        this.setState({ errorMsg: res.data.Result });
        this.setState({ PopUpBitImg: true });

        console.log(res);
      });
    } catch (e) {}
  };
  CloseModal = () => {
    this.setState({ PopUpBitImg: false });
    this.setState({ preview: profilelogo });
    this.props.onHide();
  };
  handleSliderChange = (event, NewValue) => {
    this.setState({ silderValue: NewValue });
  };
  render() {
    const PrettoSlider = withStyles({
      root: {
        color: "#2a2e2c",
        height: 5,
      },
      thumb: {
        height: 24,
        width: 24,
        backgroundColor: "#fff",
        border: "2px solid currentColor",
        marginTop: -8,
        marginLeft: -12,
        "&:focus, &:hover, &$active": {
          boxShadow: "inherit",
        },
      },
      active: {},
      valueLabel: {
        left: "calc(-50% + 4px)",
      },
      track: {
        height: 5,
        borderRadius: 4,
      },
      rail: {
        height: 5,
        borderRadius: 4,
      },
    })(Slider);
    return (
      <Modal
        show={this.props.show}
        onHide={this.props.onHide}
        backdrop={"static"}
        keyboard={"false"}
        centered
        dialogClassName="CustomWidthPic"
      >
        <div
          className="modal-content"
          style={{ width: "100%", boxShadow: "1px 5px 17px 5px #acabb1" }}
        >
          <Modal.Header closeButton style={{ border: "0px" }}>
            <Modal.Title>{this.props.Title}</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            {this.state.preview == null
              ? this.setState({ preview: profilelogo })
              : null}
            <div
              className="col-sm-12"
              style={{
                display: "flex",
                paddingTop: "20px",
              }}
            >
              <img
                className="col-sm-4"
                src={this.state.preview}
                alt="Preview"
                style={{
                  height: "120px",
                  position: "relative",
                  left: "-35px",
                  top: "-40px",
                  paddingLeft: "40px",
                  paddingRight: "40px",
                }}
              />
              <div
                style={{ position: "relative", left: "-160px", top: "90px" }}
              >
                <h6 style={{ width: "80px" }}>130 x 130</h6>
              </div>
              <div
                className="col-sm-8"
                style={{
                  position: "relative",
                  backgroundColor: "lightgrey",
                  borderRadius: "15px",
                  padding: "45px",
                  height: "230px",
                  left: "-90px",
                }}
              >
                <div
                  style={{
                    cursor: "pointer",
                    borderRadius: "15px",
                    backgroundColor: "#B8B8B8",
                    position: "absolute",
                    left: "50%",
                    top: "50%",
                    transform: "translate(-50%, -50%)",
                    backgroundImage: `url(${NoPic})`,
                  }}
                >
                  <Avatar
                    width={450}
                    height={250}
                    onCrop={this.onCrop}
                    onClose={this.onClose}
                    onBeforeFileLoad={this.onBeforeFileLoad}
                    src={this.state.src}
                    label="Click To Find"
                    cropRadius={this.state.silderValue}
                    minCropRadius="30"
                    labelStyle={{
                      color: "gray",
                      fontSize: "25PX",
                      fontWeight: "bold",
                    }}
                    backgroundColor="lightgrey"
                    onChange={this.cropRadius}
                  />
                </div>

                <div
                  style={{
                    position: "relative",
                    top: "200px",
                    display: "flex",
                  }}
                >
                  <i
                    class="fa fa-picture-o"
                    style={{ fontSize: "20px", padding: "5px" }}
                    aria-hidden="true"
                  ></i>
                  <PrettoSlider
                    valueLabelDisplay="auto"
                    aria-label="pretto slider"
                    defaultValue={10}
                    onChange={this.handleSliderChange}
                    value={
                      typeof this.state.silderValue === "number"
                        ? this.state.silderValue
                        : 0
                    }
                  />
                  <i
                    class="fa fa-picture-o"
                    style={{ fontSize: "20px", padding: "5px" }}
                    aria-hidden="true"
                  ></i>
                </div>
              </div>
            </div>
          </Modal.Body>
          <Modal.Footer style={{ border: "0px", paddingTop: "0px" }}>
            <div className="w-100 text-left ">
              <Button className="btn-black" onClick={this.imgUpoad}>
                Upload
              </Button>
            </div>
          </Modal.Footer>

          <PopUpModal
            Title={this.state.Title}
            errorMsgs={this.state.errorMsg}
            show={this.state.PopUpBitImg}
            onHide={this.CloseModal}
          />
        </div>
      </Modal>
    );
  }
}

export default UserImagePreview;
