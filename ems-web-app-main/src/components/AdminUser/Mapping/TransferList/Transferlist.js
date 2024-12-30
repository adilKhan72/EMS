import React, { Component } from "react";
import { makeStyles } from "@material-ui/core/styles";
import Grid from "@material-ui/core/Grid";
import List from "@material-ui/core/List";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import Checkbox from "@material-ui/core/Checkbox";
import Button from "@material-ui/core/Button";
import Paper from "@material-ui/core/Paper";
import "./TransferList.css";
import axios from "axios";
import Cookies from "js-cookie";
import Loader from "../../../Loader/Loader";
import Tooltip from "@material-ui/core/Tooltip";
const classes = makeStyles((theme) => ({
  root: {
    margin: "auto",
  },
  paper: {
    width: 270,
    height: 320,
    overflow: "auto",
  },
  button: {
    margin: theme.spacing(0.5, 0),
  },
}));

function not(a, b) {
  return a.filter((value) => b.indexOf(value) === -1);
}

function intersection(a, b) {
  return a.filter((value) => b.indexOf(value) !== -1);
}
class Transferlist extends Component {
  constructor(props) {
    super(props);
    this.state = {
      checked: [],
      left: [],
      right: [],
      Middle: [],
      leftChecked: [],
      rightChecked: [],
      MiddleChecked: [],
      //MappedUserIDs: [],
    };
  }
  componentDidMount() {
    if (
      this.props.lstUserMapped != undefined &&
      this.props.lstProjectUser != undefined
    ) {
      this.setState({
        left: this.props.lstUserMapped,
        right: this.props.lstProjectUser,
      });
    } else if (
      this.props.lstProjectMapped != undefined &&
      this.props.lstProjectNotMapped != undefined
    ) {
      this.setState({
        left: this.props.lstProjectNotMapped,
        right: this.props.lstProjectMapped,
      });
    } else if (
      this.props.lstClientProjectMapped != undefined &&
      this.props.lstClientProjectNotMapped != undefined
    ) {
      this.setState({
        left: this.props.lstClientProjectNotMapped,
        right: this.props.lstClientProjectMapped,
      });
    } else if (
      this.props.lstDepartmentMapped != undefined &&
      this.props.lstDepartmentNotMapped != undefined
    ) {
      if (
        this.props.AdditionalMappedList.length > 0 &&
        this.props.CheckBoxStatus == true
      ) {
        this.setState({
          Middle: this.props.AdditionalMappedList,
        });
      }
      this.setState({
        left: this.props.lstDepartmentNotMapped,
        right: this.props.lstDepartmentMapped,
      });
    }
  }
  handleToggle = (value) => () => {
    const currentIndex = this.state.checked.indexOf(value);
    const newChecked = [...this.state.checked];

    if (currentIndex === -1) {
      newChecked.push(value);
    } else {
      newChecked.splice(currentIndex, 1);
    }
    this.setState({ checked: newChecked }, () => {
      var leftState = intersection(this.state.checked, this.state.left);
      this.setState({ leftChecked: leftState });
      var RightState = intersection(this.state.checked, this.state.right);
      this.setState({ rightChecked: RightState });
    });
  };
  handleToggleMiddle = (value) => () => {
    const currentIndex = this.state.checked.indexOf(value);
    const newChecked = [...this.state.checked];
    value.AdditionalCheck = false;
    if (currentIndex === -1) {
      newChecked.push(value);
    } else {
      newChecked.splice(currentIndex, 1);
    }
    this.setState({ checked: newChecked }, () => {
      var leftState = intersection(this.state.checked, this.state.left);
      this.setState({ leftChecked: leftState });
      var MiddleState = intersection(this.state.checked, this.state.Middle);
      this.setState({ MiddleChecked: MiddleState });
    });
  };
  handleAllRight = () => {
    this.setState({ right: this.state.right.concat(this.state.left) });

    this.setState({ left: [] });
  };

  handleCheckedRight = () => {
    this.setState(
      { right: this.state.right.concat(this.state.leftChecked) },
      () => {
        this.setState({ leftChecked: [] });
      }
    );
    this.setState({ left: not(this.state.left, this.state.leftChecked) });
    this.setState({ checked: not(this.state.checked, this.state.leftChecked) });
  };

  handleCheckedLeft = () => {
    this.setState(
      { left: this.state.left.concat(this.state.rightChecked) },
      () => {
        this.setState({ rightChecked: [] });
      }
    );
    this.setState({ right: not(this.state.right, this.state.rightChecked) });
    this.setState({
      checked: not(this.state.checked, this.state.rightChecked),
    });
  };

  handleAllLeft = () => {
    if (this.props.CheckComponent == "DepartmentMapped") {
      var RightMapp = [];
      for (let i = 0; i < this.state.right.length; i++) {
        var obj = {
          Id: this.state.right[i]?.MaintaskID,
          IsActive: true,
          Name: this.state.right[i]?.Name,
        };
        RightMapp.push(obj);
      }
      if (RightMapp.length > 0) {
        this.setState({ left: this.state.left.concat(RightMapp) });
        this.setState({ right: [] });
      }

      var MiddleMapp = [];
      for (let i = 0; i < this.state.Middle.length; i++) {
        var obj = {
          Id: this.state.Middle[i]?.MaintaskID,
          IsActive: true,
          Name: this.state.Middle[i]?.Name,
        };
        MiddleMapp.push(obj);
      }
      if (MiddleMapp.length > 0) {
        this.setState({ left: this.state.left.concat(MiddleMapp) });
        this.setState({ Middle: [] });
      }
    } else {
      var RightMapp = [];
      for (let i = 0; i < this.state.right.length; i++) {
        var obj = {
          Id: this.state.right[i]?.MaintaskID,
          IsActive: true,
          Name: this.state.right[i]?.Name,
        };
        RightMapp.push(obj);
      }
      this.setState({ left: this.state.left.concat(RightMapp) });
      this.setState({ right: [] });
    }
  };
  handleAllRightMiddle = () => {
    var MiddleMapp = [];
    for (let i = 0; i < this.state.left.length; i++) {
      var obj = {
        DepartID: parseInt(this.props.DepartmentId, 10),
        DepartMappingID: 0,
        MaintaskID: this.state.left[i]?.Id,
        Name: this.state.left[i]?.Name,
        AdditionalCheck: true,
      };
      MiddleMapp.push(obj);
    }
    this.setState({ Middle: this.state.Middle.concat(MiddleMapp) });

    this.setState({ left: [] });
  };

  handleAllLeftMiddle = () => {
    var MiddleMapp = [];
    for (let i = 0; i < this.state.Middle.length; i++) {
      var obj = {
        DepartID: parseInt(this.props.DepartmentId, 10),
        DepartMappingID: 0,
        MaintaskID: this.state.Middle[i]?.Id,
        Name: this.state.Middle[i]?.Name,
        AdditionalCheck: true,
      };
      MiddleMapp.push(obj);
    }
    this.setState({ left: this.state.Middle.concat(MiddleMapp) });

    this.setState({ Middle: [] });
  };

  handleCheckedRightMiddle = () => {
    var MiddleMapp = [];
    for (let i = 0; i < this.state.leftChecked.length; i++) {
      var obj = {
        DepartID: parseInt(this.props.DepartmentId, 10),
        DepartMappingID: 0,
        MaintaskID: this.state.leftChecked[i]?.Id,
        Name: this.state.leftChecked[i]?.Name,
        AdditionalCheck: true,
      };
      MiddleMapp.push(obj);
    }
    this.setState({ Middle: this.state.Middle.concat(MiddleMapp) }, () => {
      this.setState({ leftChecked: [] });
    });

    this.setState({ left: not(this.state.left, this.state.leftChecked) });
    this.setState({ checked: not(this.state.checked, this.state.leftChecked) });
  };
  handleCheckedLeftMiddle = () => {
    this.setState(
      { left: this.state.left.concat(this.state.rightChecked) },
      () => {
        this.setState({ rightChecked: [] });
      }
    );
    this.setState({ right: not(this.state.right, this.state.rightChecked) });
    this.setState({
      checked: not(this.state.checked, this.state.rightChecked),
    });
  };
  handleCheckedMiddle = () => {
    var MiddleMapp = [];
    for (let i = 0; i < this.state.MiddleChecked.length; i++) {
      var obj = {
        Id: this.state.MiddleChecked[i]?.MaintaskID,
        IsActive: true,
        Name: this.state.MiddleChecked[i]?.Name,
      };
      MiddleMapp.push(obj);
    }
    this.setState({ left: this.state.left.concat(MiddleMapp) }, () => {
      this.setState({ MiddleChecked: [] });
    });
    this.setState({ Middle: not(this.state.Middle, this.state.MiddleChecked) });
    this.setState({
      checked: not(this.state.checked, this.state.MiddleChecked),
    });
  };
  customList = (items) => (
    <Paper className={classes.paper}>
      <List dense component="div" role="list">
        {items.map((value) => {
          const labelId = `transfer-list-item-${value.Name}-label`;

          return (
            <ListItem
              key={value}
              role="listitem"
              button
              onClick={this.handleToggle(value)}
            >
              {Cookies.get("Role") === "SuperAdmin" ? (
                <ListItemIcon>
                  <Checkbox
                    checked={this.state.checked.indexOf(value) !== -1}
                    tabIndex={-1}
                    disableRipple
                    inputProps={{ "aria-labelledby": labelId }}
                  />
                </ListItemIcon>
              ) : (
                ""
              )}
              <ListItemText id={labelId} primary={`${value.Name}`} />
            </ListItem>
          );
        })}
        <ListItem />
      </List>
    </Paper>
  );
  customListMiddle = (items) => (
    <Paper className={classes.paper}>
      <List dense component="div" role="list">
        {items.map((value) => {
          const labelId = `transfer-list-item-${value.Name}-label`;

          return (
            <ListItem
              key={value}
              role="listitem"
              button
              onClick={this.handleToggleMiddle(value)}
            >
              {Cookies.get("Role") === "SuperAdmin" ? (
                <ListItemIcon>
                  <Checkbox
                    checked={this.state.checked.indexOf(value) !== -1}
                    tabIndex={-1}
                    disableRipple
                    inputProps={{ "aria-labelledby": labelId }}
                  />
                </ListItemIcon>
              ) : (
                ""
              )}
              <ListItemText id={labelId} primary={`${value.Name}`} />
            </ListItem>
          );
        })}
        <ListItem />
      </List>
    </Paper>
  );
  MappedFun = () => {
    if (this.props.CheckComponent == "ProjectMapped") {
      this.ProjectMapping();
    } else if (this.props.CheckComponent == "MainTaskMapped") {
      this.MainTaskMapping();
    } else if (this.props.CheckComponent == "ClientMapped") {
      this.ClientProjectMapping();
    } else if (this.props.CheckComponent == "DepartmentMapped") {
      this.MaintaskDepartmentMapping();
    }
  };
  MaintaskDepartmentMapping = () => {
    try {
      var temp = [];

      // if (this.state.right.length > 0 || this.state.Middle.length > 0) {
      this.state.right.map((item) => {
        if (item.MaintaskID != null) {
          const data = {
            MainTaskIDs:
              item.Id == undefined
                ? parseInt(item.MaintaskID)
                : parseInt(item.Id),
            IsActive: false,
          };
          temp.push(data);
        } else {
          const data = {
            MainTaskIDs:
              item.Id == undefined
                ? parseInt(item.MaintaskID)
                : parseInt(item.Id),
            IsActive: false,
          };
          temp.push(data);
        }
      });
      if (this.state.Middle.length > 0) {
        this.state.Middle.map((item) => {
          if (item.MaintaskID != null) {
            const data = {
              MainTaskIDs:
                item.Id == undefined
                  ? parseInt(item.MaintaskID)
                  : parseInt(item.Id),
              IsActive: true,
            };
            temp.push(data);
          } else {
            const data = {
              MainTaskIDs:
                item.Id == undefined
                  ? parseInt(item.MaintaskID)
                  : parseInt(item.Id),
              IsActive: true,
            };
            temp.push(data);
          }
        });
        this.setState({ MappedUserIDs: temp });
      }

      const MappedObj = {
        MainTaskList: temp,
        DepartmentID: this.props.DepartmentId,
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}DepartmentMapping/DepartmentMaintaskInsertUpdate`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: MappedObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        //  this.setState({  ShowProjectName: "All Project" });
        window.location.reload(false);
      });
      // }
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ProjectMapping = () => {
    try {
      if (this.state.right.length > 0) {
        var temp = [];
        this.state.right.map((item) => {
          if (item.UserId == undefined) {
            temp.push(item.UserProfileTableID);
          } else {
            temp.push(parseInt(item.UserId));
          }
        });
        this.setState({ MappedUserIDs: temp });
      }
      const MappedObj = {
        ProjectID: this.props.ProjectID,
        UserID: temp,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/SaveProjectMapping`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: MappedObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        //  this.setState({  ShowProjectName: "All Project" });
        window.location.reload(false);
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  MainTaskMapping = () => {
    try {
      if (this.state.right.length > 0) {
        var temp = [];
        this.state.right.map((item) => {
          if (item.ProjectId == undefined) {
            temp.push(item.ID);
          } else {
            temp.push(parseInt(item.ProjectId));
          }
        });
        //this.setState({ MappedUserIDs: temp });
      }
      const MappedObj = {
        MainTaskID: this.props.MainTaskId,
        ProjectId: temp,
      };
      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/SaveMainTaskMapping`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: MappedObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        window.location.reload(false);
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  ClientProjectMapping = () => {
    try {
      if (this.state.right.length > 0) {
        var temp = [];
        this.state.right.map((item) => {
          if (item.ID != null) {
            temp.push(item.ID);
          } else {
            temp.push(parseInt(item.ID));
          }
        });
        this.setState({ MappedUserIDs: temp });
      }
      const MappedObj = {
        ProjectId: temp,
        UserID: this.props.ClientId,
      };

      axios({
        method: "post",
        url: `${process.env.REACT_APP_BASE_URL}ResourceMapping/SaveUserMapping`,
        headers: {
          Authorization: "Bearer " + localStorage.getItem("access_token"),
          encrypted: localStorage.getItem("EncryptedType"),
          Role_Type: Cookies.get("Role"),
        },
        data: MappedObj,
      }).then((res) => {
        if (res.data?.StatusCode === 401) {
          this.logout();
        }
        //  this.setState({  ShowProjectName: "All Project" });
        window.location.reload(false);
      });
    } catch (e) {
      window.location.href = "/Error";
    }
  };
  render() {
    return (
      <div>
        <Grid
          container
          spacing={2}
          justify="center"
          alignItems="center"
          className={classes.root}
        >
          <Grid item>
            {this.props.CheckComponent == "MainTaskMapped" ? (
              <h2 className="mapping_placeholder_title">Project</h2>
            ) : this.props.CheckComponent == "ClientMapped" ? (
              <h2 className="mapping_placeholder_title">Project</h2>
            ) : this.props.CheckComponent == "DepartmentMapped" ? (
              <h2 className="mapping_placeholder_title">MainTask</h2>
            ) : (
              <h2 className="mapping_placeholder_title">User</h2>
            )}

            {this.customList(this.state.left)}
          </Grid>
          {Cookies.get("Role") === "SuperAdmin" ? (
            <Grid item>
              <Grid container direction="column" alignItems="center">
                <Button
                  variant="outlined"
                  size="small"
                  className={classes.button}
                  onClick={this.handleAllRight}
                  disabled={this.state.left.length === 0}
                  aria-label="move all right"
                >
                  ≫
                </Button>
                <Button
                  variant="outlined"
                  size="small"
                  className={classes.button}
                  onClick={this.handleCheckedRight}
                  disabled={this.state.leftChecked.length === 0}
                  aria-label="move selected right"
                >
                  &gt;
                </Button>
                <Button
                  variant="outlined"
                  size="small"
                  className={classes.button}
                  onClick={this.handleCheckedLeft}
                  disabled={this.state.rightChecked.length === 0}
                  aria-label="move selected left"
                >
                  &lt;
                </Button>
                <Button
                  variant="outlined"
                  size="small"
                  className={classes.button}
                  onClick={this.handleAllLeft}
                  disabled={this.state.right.length === 0}
                  aria-label="move all left"
                >
                  ≪
                </Button>
                {this.props.CheckBoxStatus == true ? (
                  <>
                    <Tooltip title="Move All to Additional" arrow>
                      <Button
                        variant="outlined"
                        size="small"
                        className={classes.button}
                        onClick={this.handleAllRightMiddle}
                        disabled={this.state.left.length === 0}
                        aria-label="move all left"
                      >
                        ≫<i class="fa fa-plus" aria-hidden="true"></i>
                      </Button>
                    </Tooltip>
                    <Tooltip title="Move to Additional" arrow>
                      <Button
                        variant="outlined"
                        size="small"
                        className={classes.button}
                        onClick={this.handleCheckedRightMiddle}
                        disabled={this.state.leftChecked.length === 0}
                        aria-label="move selected right"
                      >
                        &gt;<i class="fa fa-plus" aria-hidden="true"></i>
                      </Button>
                    </Tooltip>
                    <Tooltip title="Remove All from Additional" arrow>
                      <Button
                        variant="outlined"
                        size="small"
                        className={classes.button}
                        onClick={this.handleCheckedMiddle}
                        disabled={this.state.MiddleChecked.length === 0}
                        aria-label="move selected left"
                      >
                        &lt;<i class="fa fa-plus" aria-hidden="true"></i>
                      </Button>
                    </Tooltip>
                    <Tooltip title="Remove from Additional" arrow>
                      <Button
                        variant="outlined"
                        size="small"
                        className={classes.button}
                        onClick={this.handleAllLeftMiddle}
                        disabled={this.state.Middle.length === 0}
                        aria-label="move all left"
                      >
                        ≪<i class="fa fa-plus" aria-hidden="true"></i>
                      </Button>
                    </Tooltip>
                  </>
                ) : null}
              </Grid>
            </Grid>
          ) : (
            ""
          )}
          <Grid item>
            {this.props.CheckComponent == "MainTaskMapped" ? (
              <h2 className="mapping_placeholder_title">Mapped Projects</h2>
            ) : this.props.CheckComponent == "ClientMapped" ? (
              <h2 className="mapping_placeholder_title">Mapped Project</h2>
            ) : this.props.CheckComponent == "DepartmentMapped" ? (
              <h2 className="mapping_placeholder_title">MainTask Mapped</h2>
            ) : (
              <h2 className="mapping_placeholder_title">Mapped User</h2>
            )}

            {this.customList(this.state.right)}
          </Grid>
          {this.props.CheckBoxStatus == true ? (
            <Grid item>
              <h2 className="mapping_placeholder_title">Additional MainTask</h2>
              {this.customListMiddle(this.state.Middle)}
            </Grid>
          ) : null}
        </Grid>
        <div className="col-sm-12 text-right">
          {Cookies.get("Role") === "SuperAdmin" ? (
            <button
              onClick={this.MappedFun}
              type="button"
              className="btn-black mapped_submit"
            >
              Submit <i className="menu-icon fa fa-check"></i>
            </button>
          ) : (
            ""
          )}
        </div>
      </div>
    );
  }
}
export default Transferlist;
